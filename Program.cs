using System;
using System.Collections.Generic;

namespace Lab8_OS_Dimchuk
{
    class Program
    {
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


            //start at index 2 to skip algorithm type & size of memory
            for(var i = 2; i < text.Length; i++)
            {
                DetermineFunction(text[i]);
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
                    GenericDeallocation(processId);

                }
                else if (function == "A") //option 1 - allocate a memory with id PID, that's size MEMORY_SIZE. Unit would be in KBs.
                {
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

            if (memory.First == null)
            {
                nodeToMake.startIndex = 0;
                memory.AddFirst(nodeToMake);
            }
            else
            {
                //int index = memory.First.Value.size; //first valid index after 1st node

                int index = 0;
                var current = memory.First;
                for(int i = 0; i < memory.Count; i++)
                {
                    nodeToMake.startIndex = index;

                    //when empty block in middle, check size
                    if (!current.Value.full)
                    {
                        if (current.Next != null)
                        {
                            //this will be broken if you dont merge empty blocks
                            var emptyBlockSize = current.Value.startIndex + current.Next.Value.startIndex;
                            if (emptyBlockSize <= size) //if enough space, replace, otherwise move on
                            {
                                //replace this node
                                memory.AddBefore(current, nodeToMake);
                                memory.Remove(current);
                            }
                        }
                        else if (filledSize <= sizeKB)
                        {
                            //replace this node
                            memory.AddBefore(current, nodeToMake);
                            memory.Remove(current);
                        }
                        else Console.WriteLine("ERROR - adding first, because ran out of size?");
                    }

                    //TODO missing here, adding a new node if we got to the very end/...



                    if(current.Value.size == 0)
                    {
                        //this is an empty block in the middle
                        if (current.Next?.Value != null && current.Next.Value.startIndex != 0)
                        {
                            index += current.Next.Value.startIndex;
                        }
                        else //this is an empty block at the end 
                        {
                            index += (sizeKB - filledSize);
                        }
                    }
                    else
                    {
                        index += current.Value.size;
                    }

                    current = current.Next;
                }

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
            Console.WriteLine("PRINTING...");
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
