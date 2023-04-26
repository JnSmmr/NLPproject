
public class Program{
    static void Main (string[] args){
        String filePath = "../tests/data/training.mrg";

        SExpressionParser parser = new SExpressionParser();
        Grammar grammar = parser.parseFile(filePath);

        Console.WriteLine("something: " + grammar.rules.Count());
    }
}
    //TODO: put struct classes in different file, are structs better maybe?

public class SExpressionParser{
    private String[] tokens = new String[]{};
    int counter;

    GrammarBuilder grammarBuilder = new GrammarBuilder();

    public Grammar parseFile(String file){

        IEnumerable<string> lines = File.ReadLines(file);
        foreach ( string line in lines){
            tokens = tokenizeExpression(line).ToArray();
            counter=2;
            Tree parsedTree = parseSExpression();
            grammarBuilder.induceGrammar(parsedTree);
        }
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
    public Grammar grammar = new Grammar();

    public void induceGrammar(Tree tree){
        
        List<Rule> rules = formulateRules(tree);
        foreach( Rule rule in rules){
            grammar.addRule(rule);           
        }
    }

    static public List<Rule> formulateRules(Tree tree){
        List<Rule> rules = new List<Rule>();

        if (tree.children.Count == 1){
            String[] sList = new String[]{tree.children[0].value};
            Rule lexRule = new Rule(tree.value,sList);
            rules.Add(lexRule);
            return rules;
        }
        else{
            List<String> children = new List<string>();
            for (int i = 0; i<tree.children.Count(); i++){
                children.Add(tree.children[i].value);
                rules.AddRange(formulateRules(tree.children[i]));
            } 
            Rule nonLexRule = new Rule(tree.value, children.ToArray());
            rules.Add(nonLexRule);
            return rules;            
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

    bool isLexical = false;
    public String leftSide;
    public String[] rightSide;

    public Rule(String ls, String[] rs){
        leftSide = ls;       
        rightSide = rs;
    }

    public override bool Equals(object? obj)
    {
        return obj is Rule rule &&
               isLexical == rule.isLexical &&
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
        return HashCode.Combine(isLexical, leftSide);
    }
}


public class Grammar{
    public Dictionary<Rule, float> rules = new Dictionary<Rule, float>();
    public Dictionary<string, int> leftSideCount = new Dictionary<string, int>();

    public void addRule(Rule rule){
        
        if (rules.ContainsKey(rule)){
            rules[rule] ++;
        }
        else{
            rules.Add(rule,1);
        }          
        
        if ( leftSideCount.ContainsKey(rule.leftSide)){
            leftSideCount[rule.leftSide]++;
        }
        else{
            leftSideCount[rule.leftSide] = 1;
        }
    }

    public void NormalizeRules(){
        foreach (Rule rule in rules.Keys.ToList()){
            int occurenceCount = leftSideCount[rule.leftSide];
            float normalizedRuleWeight = rules[rule] / occurenceCount;
            rules[rule] = normalizedRuleWeight;
        }
    }
}