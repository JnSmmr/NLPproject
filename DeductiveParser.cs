using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLPproject
{
  
    public class DeductiveParser
    {

        public void initialize(string grammarPath, string lexiconPath){
            //Read PCFG
            using (StreamReader reader = new StreamReader(lexiconPath)){
                string? line;

                while ((line = reader.ReadLine()) != null){
                    string[] splitLine = line.Split(" ");
                    DeductionRule rule = new DeductionRule(splitLine[0],splitLine[1]);
                    if(lexicalRules.ContainsKey(splitLine[1])){
                        lexicalRules[splitLine[1]].Add((rule,double.Parse(splitLine[2])));
                    }
                    else{
                        SortedSet<(DeductionRule,double)> sortedSet = new SortedSet<(DeductionRule, double)> 
                            (Comparer<(DeductionRule,double)>.Create((x, y) => y.Item2.CompareTo(x.Item2)));
                        lexicalRules.Add(splitLine[1],sortedSet);
                    }
                }
            }
            using (StreamReader reader = new StreamReader(grammarPath)){
                string? line;

                while ((line = reader.ReadLine()) != null){
                    string[] splitLine = line.Split(" ");
                    double weight = double.Parse(splitLine[splitLine.Length - 1]);
                    DeductionRule rule;
                    if(splitLine.Length == 4){
                        rule = new DeductionRule(splitLine[0],splitLine[2]);
                    }
                    else{
                        rule = new DeductionRule(splitLine[0],splitLine[2],splitLine[3]);  
                    }
                    for (int i = 2; i<splitLine.Length -1; i++){
                        if(lexicalRules.ContainsKey(splitLine[i])){
                            lexicalRules[splitLine[i]].Add((rule,weight));
                        }
                        else{
                        SortedSet<(DeductionRule,double)> sortedSet = new SortedSet<(DeductionRule, double)> 
                            (Comparer<(DeductionRule,double)>.Create((x, y) => y.Item2.CompareTo(x.Item2)));
                        nonlexRules.Add(splitLine[i],sortedSet);
                        }
                    }
                }           
            }

        }
        public void parse(){
            //while possible:
                //Read Line
                //apply algorithm
                //output best tree
            while(true){
                string line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)){
                    break;
                }
                deduce(line.Split(" "));
            }



        }

        public void deduce(String[] sentence){
          
          // Line 2, init Queue
          PriorityQueue<queueElement,double> queue = new PriorityQueue<queueElement,double>();
            for ( int i = 0; i < sentence.Length; i++){
                (DeductionRule,double) bestRule;                
                if (lexicalRules.TryGetValue(sentence[i], out var candidateRules)){
                    bestRule = candidateRules.Max;
                }
                else{
                    //TODO: Write ERR
                }
                //TODO: queueElement groß schreiben
                queue.Add(new queueElement(i, bestRule((key)).leftSide, bestRule((value));                
            }
            // Line 3 init c
            
                                           
            while (!queue.isEmpty()){
              
            }
            
        }


    public Dictionary<string,SortedSet<(DeductionRule,double)>> lexicalRules = new Dictionary<string, SortedSet<(DeductionRule, double)>>();    
     public Dictionary<string,SortedSet<(DeductionRule,double)>> nonlexRules = new Dictionary<string, SortedSet<(DeductionRule, double)>>();

    }

    public struct queueElement{
        int leftCounter;
        int rightCounter;
        String nonterminal;
        List<DeductionRule> backtrace;
      
        public queueElement(int l, int r, String A){
          leftCounter = l;
          rightCounter = r;
          nonterminal = A;
          }
    }

    public struct DeductionRule{
        public string leftSide;
        public string rightSideOne;
        public string? rightSideTwo;

        public DeductionRule(string leftSide, string rightSideOne){
            this.leftSide = leftSide;
            this.rightSideOne = rightSideOne;
            this.rightSideTwo = null;
        }
        public DeductionRule(string leftSide, string rightSideOne, string rightSideTwo){
            this.leftSide = leftSide;
            this.rightSideOne = rightSideOne;
            this.rightSideTwo = rightSideTwo;
        }
    } 
    
}
