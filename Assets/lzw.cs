using System;

// This is a re-implemntation in C# of the lzw algorithm used in ultima 4.
// It uses a fixed codeword length of 12 bits.
// The dictionary is implemented using a hash table.

public class lzw
{
    public struct DictionaryEntry
    {
        public byte root;
        public int codeword;
        public bool occupied;
    };

    public long GetDecompressedSize(byte[] compressedInput, long compressedSize)
    {
        return Decompress(false, compressedInput, null, compressedSize);
    }

    public long Decompress(byte[] compressedInput, byte[] decompressedOutput, long compressedSize)
    {
        return Decompress(true, compressedInput, decompressedOutput, compressedSize);
    }

    long Decompress(bool writeOutput, byte[] compressedInput, byte[] decompressedOutput, long compressedSize)
    {
        int i;

        const int maxDictionaryEntries = 0xccc;

        const int stackSize = 0x8000;
        const int dictionarySize = 0x1000;

        int oldCodeword;
        int newCodeword;
        byte data;

        long bitsRead = 0;
        long outputBytes = 0;

        int newPosition;
        bool unknownCodeword;

        // allocate the dictionary and index
        DictionaryEntry[] dictionary = new DictionaryEntry[dictionarySize]; 
        int codewordsInDictionary = 0;

        // allocate the stack and index
        byte[] stack = new byte[stackSize];
        int elementsInStack = 0;

        // clear the dictionary
        Array.Clear(dictionary, 0, dictionary.Length);
        for (i = 0; i < 0x100; i++)
        {
            dictionary[i].occupied = true;
        }

        // check if we are going to run out of bits, otherwise keep going
        if (bitsRead + 12 <= compressedSize * 8)
        {
            // get next codeword & data
            oldCodeword = getNextCodeword(ref bitsRead, compressedInput);
            data = (byte)(oldCodeword & 0xff);

            // check if we are actually writing data or just figuring out how big the decompression buffer needs to be
            if (writeOutput)
            {
                // write the output
                decompressedOutput[outputBytes] = data;
            }

            // keep track of output bytes
            outputBytes++;

            // check if we are going to run out of bits, otherwise keep going
            while (bitsRead + 12 <= compressedSize * 8)
            {
                // get next codeword
                newCodeword = getNextCodeword(ref bitsRead, compressedInput);

                // check if we have this codeword already in the dictionary
                if (dictionary[newCodeword].occupied)   
                {
                    // codeword is not new 
                    unknownCodeword = false;

                    // Convert the codeword into a string
                    getString(newCodeword, dictionary, ref stack, ref elementsInStack);
                }
                else
                {
                    // codeword is new
                    unknownCodeword = true;

                    // add data to stack
                    stack[elementsInStack] = data;
                    elementsInStack++;

                    // Convert the codeword into a string
                    getString(oldCodeword, dictionary, ref stack, ref elementsInStack);
                }

                // pull data off the stack
                data = stack[elementsInStack - 1];

                // output data
                while (elementsInStack > 0)
                {
                    // check if we are actually writing data or just figuring out how big the decompression buffer needs to be
                    if (writeOutput)
                    {
                        // copy stack data to output
                        decompressedOutput[outputBytes] = stack[elementsInStack - 1];
                    }

                    // keep track of output bytes
                    outputBytes++;

                    // keep track of elements in stack
                    elementsInStack--;
                }

                // get hash codeword for oldcodeword & data
                newPosition = getNewHashCode(data, oldCodeword, dictionary);

                // add to dictionary
                dictionary[newPosition].root = data;
                dictionary[newPosition].codeword = oldCodeword;
                dictionary[newPosition].occupied = true;
                codewordsInDictionary++;

                // simple check for errors
                if (unknownCodeword && (newPosition != newCodeword))
                {
                    // something is wrong
                    return -1;
                }

                // check if the dictionary is full
                if (codewordsInDictionary > maxDictionaryEntries)
                {
                    // clear the dictionary
                    codewordsInDictionary = 0;
                    Array.Clear(dictionary, 0, dictionary.Length);
                    for (i = 0; i < 0x100; i++)
                    {
                        dictionary[i].occupied = true;
                    }

                    // check if we are going to run out of bits, otherwise keep going
                    if (bitsRead + 12 <= compressedSize * 8)
                    {
                        // get next codeword & data
                        newCodeword = getNextCodeword(ref bitsRead, compressedInput);
                        data = (byte)newCodeword;

                        // check if we are actually writing data or just figuring out how big the decompression buffer needs to be
                        if (writeOutput)
                        {
                            // copy data to output
                            decompressedOutput[outputBytes] = data;
                        }

                        // keep track of output bytes
                        outputBytes++;
                    }
                    else
                    {
                        // done
                        return outputBytes;
                    }
                }

                // save oldCodeword
                oldCodeword = newCodeword;
            } 
        }

        // done
        return outputBytes;
    }

    int getNextCodeword(ref long bitsRead, byte[] compressedInput)
    {
        // get the next 12-bit codeword
        int codeword = (compressedInput[bitsRead / 8] << 8) + compressedInput[bitsRead / 8 + 1];
        codeword = codeword >> (4 - (int)(bitsRead % 8));
        codeword = codeword & 0xfff;

        // keep track of bits read
        bitsRead += 12;

        return codeword;
    }

    void getString(int codeword, DictionaryEntry[] dictionary, ref byte[] stack, ref int elementsInStack)
    {
        byte root;
        int currentCodeword = codeword;

        while (currentCodeword > 0xff)
        {
            root = dictionary[currentCodeword].root;
            currentCodeword = dictionary[currentCodeword].codeword;
            stack[elementsInStack] = root;
            elementsInStack++;
        }

        // add data inside codeword to stack
        stack[elementsInStack] = (byte)(currentCodeword & 0xff);
        elementsInStack++;
    }

    int getNewHashCode(byte root, int codeword, DictionaryEntry[] dictionary)
    {
        int hashCode;

        hashCode = hashType1(root, codeword);
        if (hashPositionFound(hashCode, root, codeword, dictionary))
        {
            return hashCode;
        }

        hashCode = hashType2(root, codeword);
        if (hashPositionFound(hashCode, root, codeword, dictionary))
        {
            return hashCode;
        }

        do
        {
            hashCode = hashType3(hashCode);
        }
        while (!hashPositionFound(hashCode, root, codeword, dictionary));

        return hashCode;
    }

    int hashType1(byte root, int codeword)
    {
        int newHashCode = ((root << 4) ^ codeword) & 0xfff;
        return newHashCode;
    }

    int hashType2(byte root, int codeword)
    {
        long[] registers = new long[2];
        long temp;
        long carry, oldCarry;
        int i, j;

        registers[1] = 0;
        registers[0] = ((root << 1) + codeword) | 0x800;

        temp = (registers[0] & 0xff) * (registers[0] & 0xff);
        temp += 2 * (registers[0] & 0xff) * (registers[0] >> 8) * 0x100;
        registers[1] = (temp >> 16) + (registers[0] >> 8) * (registers[0] >> 8);
        registers[0] = temp & 0xffff;

        if (registers[1] == 0) 
        {
            carry = 0; 
        }
        else 
        { 
            carry = 1; 
        }

        for (i = 0; i < 2; i++)  
        {
            for (j = 0; j < 2; j++)   
            {
                oldCarry = carry;
                carry = (registers[j] >> 15) & 1;
                registers[j] = (registers[j] << 1) | oldCarry;
                registers[j] = registers[j] & 0xffff;
            }
        }

        registers[0] = ((registers[0] >> 8) | (registers[1] << 8)) & 0xfff;

        return (int)registers[0];
    }

    int hashType3(int hashCode)
    {
        const long probeOffset = 509;   // a prime number

        long newHashCode = (hashCode + probeOffset) & 0xfff;
        return (int)newHashCode;
    }

    bool hashPositionFound(int hashCode, byte root, int codeword, DictionaryEntry[] dictionary)
    {
        // hash codes must not be roots
        if (hashCode > 0xff)   
        {
            bool hashTablePostionOccupied;
            bool hashTableEntryMatches;

            // check hash table position
            if (dictionary[hashCode].occupied)
            {
                // hash table position is occupied
                hashTablePostionOccupied = true;

                // is (root, codeword) pair already in the hash table, then we will ignore
                hashTableEntryMatches = (dictionary[hashCode].root == root) && (dictionary[hashCode].codeword == codeword);
            }
            else
            {
                // hash table position is unoccupied
                hashTablePostionOccupied = false;
                hashTableEntryMatches = false;
            }

            return !hashTablePostionOccupied || hashTableEntryMatches;
        }
        else
        {
            return false;
        }
    }
}