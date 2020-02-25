using System;
using System.Collections.Generic;

namespace Lab8_OS_Dimchuk
{
    class Program
    {
        public static int NO_PROCESS = -1;

        static AlgorithmType type;
        static int sizeKB;
        public static LinkedList<Node> memory = new LinkedList<Node>();

        static int filledSize = 0;

        static void Main(string[] args)
        {

            Console.WriteLine("Please enter path for file of text instructions: ");
            //var path = Console.ReadLine();
            var path = @"C:\Users\Daria\Documents\BCIT\CST\Term 4\OS\lab8-MM-assignment-java\test2.txt";

            var text = System.IO.File.ReadAllLines(@path);

            
            type = Convert(int.Parse(text[0]));
            sizeKB = int.Parse(text[1]);

            memory.AddFirst(new Node { startIndex = 0, size = sizeKB, processId = NO_PROCESS, full = false });

            Console.WriteLine("\nBeginning of program:");
            PrintDetails();


            //start at index 2 to skip algorithm type & size of memory
            for (var i = 2; i < text.Length; i++)
            {
                DetermineFunction(text[i]);
                PrintDetails();
            }
        }




        static void DetermineFunction(string val)
        {
            var instructions = val.ToUpper().Split(' ');

            var function = instructions[0];

            //option 3 - print out current state of memory (allocations and free spaces)
            if (function == "P")
            {
                PrintDetails();
            }
            else
            {
                int processId = int.Parse(instructions[1]);

                //option 2 - deallocate memory for process with id PID
                if (function == "D")
                {
                    Console.WriteLine("\nDeallocating process " + processId);
                    GenericDeallocation(processId);

                }
                else if (function == "A") //option 1 - allocate a memory with id PID, that's size MEMORY_SIZE. Unit would be in KBs.
                {
                    Console.WriteLine("\nAllocating for process " + processId);
                    int size = int.Parse(instructions[2]);
                    GenericAllocation(processId, size);
                }
            }
        }

        

        /*      ALLOCATION METHODS      */

        static void Allocate_First(int id, int size)
        {
            filledSize += size;

            var nodeToMake = new Node { processId = id, size = size, full = true};

            //int index = memory.First.Value.size; //first valid index after 1st node

            var success = false;

            //completely empty linkedlist
            if(memory.First.Next == null)
            {
                nodeToMake.startIndex = 0;
                memory.AddBefore(memory.First, nodeToMake);
                memory.First.Next.Value.startIndex = nodeToMake.size + 1;
                memory.First.Next.Value.size -= nodeToMake.size + 1;
                success = true;
            }
            else
            {
                var current = memory.First;
                while(current != null)
                {
                    nodeToMake.startIndex = current.Value.startIndex;

                    //if empty and fits our size
                    if (!current.Value.full && current.Value.size >= size)
                    {
                        memory.AddBefore(current, nodeToMake);

                        current.Value.startIndex = nodeToMake.size + 1 + nodeToMake.startIndex;
                        current.Value.size -= nodeToMake.size + 1;

                        success = true;
                        break;
                    }

                    current = current.Next;
                }
            }


            if (!success)
            {
                Console.WriteLine("OOps, we ran out of spacE?? " + filledSize + " / " + sizeKB);
                //maybe add call do do compaction???
            }
        }

        static void Allocate_Best(int id, int size)
        {
            filledSize += size;

        }


        static void Allocate_Worst(int id, int size)
        {
            filledSize += size;

        }

        /*      DEALLOCATION METHODS      */

        static void Deallocate_First(int id)
        {
            //filledSize -= size;
            //always check the neighbours to see if you can merge empty blocks

            var current = memory.First;
            while(current != null)
            {
                //found!
                if (current.Value.processId == id)
                {
                    current.Value.full = false;
                    current.Value.processId = NO_PROCESS;
                    filledSize -= current.Value.size;



                    var leftMergeNeeded = current.Previous != null && !current.Previous.Value.full;
                    var rightMergeNeeded = current.Next != null && !current.Next.Value.full;
                    var bothSidesMergeNeeded = leftMergeNeeded && rightMergeNeeded;

                    var anyMergeNeeded = leftMergeNeeded | rightMergeNeeded;

                    if (anyMergeNeeded)
                    {
                        //in case we need to merge some empty blocks together
                        var start = 0;
                        var size = 0;
                        LinkedListNode<Node> nodeToAddBeforeTo = null;
                        var nodesToRemove = new List<LinkedListNode<Node>>();

                        //if both neighbours on each side is empty, remove and merge sides
                        if (bothSidesMergeNeeded)
                        {
                            start = current.Previous.Value.startIndex;
                            size = current.Previous.Value.size + current.Value.size + current.Next.Value.size;

                            nodeToAddBeforeTo = current.Previous;
                            nodesToRemove.Add(current.Previous);
                            nodesToRemove.Add(current.Next);
                            nodesToRemove.Add(current);
                        }
                        //if just prev neighbour is empty
                        else if (leftMergeNeeded)
                        {
                            start = current.Previous.Value.startIndex;
                            size = current.Previous.Value.size + current.Value.size;

                            nodeToAddBeforeTo = current.Previous;
                            nodesToRemove.Add(current.Previous);
                            nodesToRemove.Add(current);
                        }
                        //if just next neighbour is empty
                        else if (rightMergeNeeded)
                        {
                            start = current.Value.startIndex;
                            size = current.Next.Value.size + current.Value.size;

                            nodeToAddBeforeTo = current;
                            nodesToRemove.Add(current.Next);
                            nodesToRemove.Add(current);
                        }


                        var node = new Node { processId = NO_PROCESS, startIndex = start, size = size, full = false };
                        memory.AddBefore(nodeToAddBeforeTo, node);

                        foreach (var rmv in nodesToRemove)
                        {
                            memory.Remove(rmv);
                        }
                    }//merging area done

                    break;
                }

                current = current.Next;
            }


        }

        static void Deallocate_Best(int id)
        {
            //filledSize -= size;
            //always check the neighbours to see if you can merge empty blocks
        }

        static void Deallocate_Worst(int id)
        {
            //filledSize -= size;
            //always check the neighbours to see if you can merge empty blocks
        }


        /*      GENERIC FUNCTIONALITY METHODS      */

        static void PrintDetails()
        {
            Console.WriteLine("\nPRINTING...");

            var current = memory.First;
            while(current != null)
            {
                var start = current.Value.startIndex;
                var end = current.Value.startIndex + current.Value.size;

                var id = current.Value.processId != NO_PROCESS ? "process " + current.Value.processId : "";
                var state = current.Value.full ? " full" : " EMPTY";
                var size = current.Value.size;

                Console.WriteLine("[" + start + " - " + end + "]" + " - " + id + state + " size: " + size);

                current = current.Next;
            }
        }


        static void GenericDeallocation(int id)
        {
            if (type == AlgorithmType.First)
            {
                Deallocate_First(id);
            }
            else if (type == AlgorithmType.Best)
            {
                Deallocate_Best(id);
            }
            else if (type == AlgorithmType.Worst)
            {
                Deallocate_Worst(id);
            }
        }



        static void GenericAllocation(int id, int size)
        {
            if (type == AlgorithmType.First)
            {
                Allocate_First(id, size);
            }
            else if (type == AlgorithmType.Best)
            {
                Allocate_Best(id, size);
            }
            else if (type == AlgorithmType.Worst)
            {
                Allocate_Worst(id, size);
            }
        }



        static AlgorithmType Convert(int val)
        {
            return val == 1 ? AlgorithmType.First : (val == 2 ? AlgorithmType.Best : AlgorithmType.Worst);
        }


        enum AlgorithmType
        {
            First = 1,
            Best = 2,
            Worst = 3
        }


        public class Node
        {
            public int processId;
            public int startIndex;
            public int size;
            public bool full;
        }
    }

}
