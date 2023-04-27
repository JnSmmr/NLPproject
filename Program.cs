
public class Program{
    static int Main (string[] args){

        Console.WriteLine(args.Length);
        if (args.Length == 0){
            Console.WriteLine("Please use arguments to specify the required task.");
            return 1;
        }
        if (args[0] == "induce"){
            String? grammarName = null;
            if (args.Length > 1){
                grammarName = args[1];
            }
            SExpressionParser parser = new SExpressionParser();
            Grammar grammar = parser.parseFile(grammarName);
            grammar.write();
            return 0;
        }


    Console.WriteLine("This feature has propably not been implemented yet.");
    return 22;
    }

    
}
    //TODO: put struct classes in different file, are structs better maybe?

public class SExpressionParser{
    private String[] tokens = new String[]{};
    int counter;



    public Grammar parseFile(String grammarName){

        GrammarBuilder grammarBuilder = new GrammarBuilder(grammarName);

        Console.WriteLine("Please input the constituent trees as s-expressions");
        List<string> lines = new List<string>();
        while(true){
            string line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line)){
                break;
            }
            lines.Add(line);
        }
        
        foreach ( string line in lines){
            tokens = tokenizeExpression(line).ToArray();
            counter=2;
            Tree parsedTree = parseSExpression();
            grammarBuilder.induceGrammar(parsedTree);
        }
        grammarBuilder.grammar.normalizeRules();
        return grammarBuilder.grammar;
    } 

    List<String> tokenizeExpression(String expr){

        List<String> tokens = new List<String>();

        String currentSubstring = "";

        foreach(char c in expr){
            if (char.IsWhiteSpace(c)){
                if (currentSubstring !=""){
                    tokens.Add(currentSubstring);
                    currentSubstring = "";
                }
            }
            else if (c == '(' ){
                tokens.Add(c.ToString());
            }
            else if ( c== ')'){
                if (currentSubstring != ""){
                    tokens.Add(currentSubstring);
                }
                tokens.Add(c.ToString());
                currentSubstring = "";
            }
            else{
                currentSubstring += c;
            }           
        }
        return tokens;
    }

    Tree parseSExpression(){
        
        string token = tokens[counter++];

        if (token == "("){
            Tree tree = new Tree(tokens[counter++]);
            while (tokens[counter] != ")"){
                tree.children.Add(parseSExpression());
                }
            counter++;
            return tree;           
        }
        else {
            return new Tree(token);
        }
    } 
}

public class GrammarBuilder{
    public Grammar grammar;
    public GrammarBuilder(String grammarName){

        grammar = new Grammar(grammarName);
    }


    public void induceGrammar(Tree tree){
        
        (List<Rule> lexRules, List<Rule> nonLexRules) = formulateRules(tree);
        foreach( Rule rule in lexRules){
            grammar.addLexRule(rule);           
        }
        foreach(Rule rule in nonLexRules){
            grammar.addNonLexRule(rule);
        }
    }

    static public (List<Rule>, List<Rule>) formulateRules(Tree tree){
        List<Rule> lexRules = new List<Rule>();
        List<Rule> nonLexRules = new List<Rule>();

        if (tree.children.Count == 1 && !tree.children[0].children.Any()){
            String[] sList = new String[]{tree.children[0].value};
            Rule lexRule = new Rule(tree.value,sList,true);
            lexRules.Add(lexRule);
            return (lexRules,nonLexRules);
        }
        else{
            List<String> children = new List<string>();
            for (int i = 0; i<tree.children.Count(); i++){
                children.Add(tree.children[i].value);
                (List<Rule>, List<Rule>) recursiveRules = formulateRules(tree.children[i]);
                lexRules.AddRange(recursiveRules.Item1);
                nonLexRules.AddRange(recursiveRules.Item2);

            } 
            Rule nonLexRule = new Rule(tree.value, children.ToArray(),false);
            nonLexRules.Add(nonLexRule);
            return (lexRules,nonLexRules);            
            }
        }
    }


public struct Tree{
    public String value;
    public List <Tree> children;
    public Tree(String s){
        value = s;
        children = new List<Tree>();
    }
}


public struct Rule{

    public String leftSide;
    public String[] rightSide;

    public Rule(String ls, String[] rs, Boolean lexical){
        leftSide = ls;       
        rightSide = rs;
    }

    public override bool Equals(object? obj)
    {
        return obj is Rule rule &&
               leftSide == rule.leftSide &&
               checkRightEq(rightSide, rule.rightSide);
    }
    public bool checkRightEq(string[] rightSide, string[] ruleRightSide){
        if(rightSide.Length != ruleRightSide.Length){
            return false;
        }
        for( int i = 0; i< rightSide.Length; i++){
            if(!(rightSide[i] == ruleRightSide[i])){
                return false;
            }
        }
        return true;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(leftSide);
    }
}


public class Grammar{
    public Dictionary<Rule, float> nonLexRules = new Dictionary<Rule, float>();
    public Dictionary<Rule, float> lexRules = new Dictionary<Rule, float>();
    public Dictionary<string, int> leftSideCount = new Dictionary<string, int>();
    public String? name;

    public Grammar(String grammarName){
        name = grammarName;
    }    

    public void addLexRule(Rule rule){
        
        if (lexRules.ContainsKey(rule)){
            lexRules[rule] ++;
        }
        else{
            lexRules.Add(rule,1);
        }          
        
        if ( leftSideCount.ContainsKey(rule.leftSide)){
            leftSideCount[rule.leftSide]++;
        }
        else{
            leftSideCount[rule.leftSide] = 1;
        }
    }
        public void addNonLexRule(Rule rule){
        
        if (nonLexRules.ContainsKey(rule)){
            nonLexRules[rule] ++;
        }
        else{
            nonLexRules.Add(rule,1);
        }          
        
        if ( leftSideCount.ContainsKey(rule.leftSide)){
            leftSideCount[rule.leftSide]++;
        }
        else{
            leftSideCount[rule.leftSide] = 1;
        }
    }

    public void normalizeRules(){
        foreach (Rule rule in lexRules.Keys.ToList()){
            int occurenceCount = leftSideCount[rule.leftSide];
            float normalizedRuleWeight = lexRules[rule] / occurenceCount;
            lexRules[rule] = normalizedRuleWeight;
        }
        foreach (Rule rule in nonLexRules.Keys.ToList()){
            int occurenceCount = leftSideCount[rule.leftSide];
            float normalizedRuleWeight = nonLexRules[rule] / occurenceCount;
            nonLexRules[rule] = normalizedRuleWeight;
        }


    }

    public void write(){
        if(name == null){
            writeToCMD();
        }
        else{
            writeToFile();
        }
    }
    public void writeToCMD(){

        SortedSet<string> setOfWords = new SortedSet<string>();
        
        Console.WriteLine("Nonlexical Rules:  _______________________________________________");
        foreach (KeyValuePair<Rule,float> weightedRule in nonLexRules){   
            String outString = (weightedRule.Key.leftSide) + " ->";
            foreach (String n in weightedRule.Key.rightSide){
                outString = outString + " " + n;
            }
            Console.WriteLine(outString + " " + weightedRule.Value);
            }
        Console.WriteLine("Lexical Rules: _______________________________________________");
        foreach (KeyValuePair<Rule,float> weightedRule in lexRules){

            Console.WriteLine(weightedRule.Key.leftSide + " " + weightedRule.Key.rightSide[0] + " " + weightedRule.Value);                
            if(!setOfWords.Contains(weightedRule.Key.rightSide[0])){
                Console.WriteLine(weightedRule.Key.leftSide + " " + weightedRule.Key.rightSide[0] + " " + weightedRule.Value); 
                setOfWords.Add(weightedRule.Key.rightSide[0]);
            }
        }
        
        Console.WriteLine("Words present: _______________________________________________");
        foreach(String word in setOfWords){
            Console.WriteLine(word);
        }       
    }
        public void writeToFile(){

        string rulesFile = name + ".rules";
        string lexiconFile = name + ".lexicon";
        string wordsFile = name + ".words";

        if(File.Exists(rulesFile)){
            File.Delete(rulesFile);
        }
        if(File.Exists(lexiconFile)){
            File.Delete(lexiconFile);
        }
        if(File.Exists(wordsFile)){
            File.Delete(wordsFile);
        }

        SortedSet<string> setOfWords = new SortedSet<string>();

        using(StreamWriter lexiconWriter = new StreamWriter(lexiconFile))
        using(StreamWriter ruleWriter = new StreamWriter(rulesFile)){

            foreach (KeyValuePair<Rule,float> weightedRule in lexRules){

                lexiconWriter.WriteLine(weightedRule.Key.leftSide + " " + weightedRule.Key.rightSide[0] + " " + weightedRule.Value);                
                if(!setOfWords.Contains(weightedRule.Key.rightSide[0])){
                    setOfWords.Add(weightedRule.Key.rightSide[0]);
                }
            }
                
            foreach (KeyValuePair<Rule,float> weightedRule in nonLexRules){
                String outString = (weightedRule.Key.leftSide) + " ->";
                foreach (String n in weightedRule.Key.rightSide){
                    outString = outString + " " + n;
                }
                outString = outString + " " + weightedRule.Value;
                ruleWriter.WriteLine(outString);                
            }
        }
 
        using (StreamWriter wordWriter = new StreamWriter(wordsFile)){
            foreach(String word in setOfWords){
                wordWriter.WriteLine(word);
            }
        }
    }
}