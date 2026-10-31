using System.Text;

namespace Gym.Day02
{
    public static class Solution
    {
        public static int[] PrefixSum(int[] nums)
        {
            if(nums.Length == 0) return nums;

            int sum = 0;

            for( int i = 0; i < nums.Length; i++)
            {
                nums[i] = sum + nums[i];
                sum = nums[i];
            }
            return nums;
        }

        public static int[] RemoveDuplicate(int[] nums)
        {
            if(nums.Length == 0) return nums;

            int cI = 0;

            for( int i = 1; i < nums.Length; i++)
            {
                if(nums[i] != nums[cI])
                {
                    Console.WriteLine($" CI = {cI}");
                    nums[cI+1] = nums[i];
                    cI++;
                    Console.WriteLine($" CI = {cI}");
                }
                
            }
            return nums[0..(cI+1)];
        }

        public static void MoveZeroes(int[] nums) {
            // [0, 1, 0, 3, 12]
            if(nums.Length > 0)
            {
                int writePointer = 0;

                for(int i = 0; i < nums.Length; i++)
                {
                    if(nums[i] != 0)
                    {
                        nums[writePointer] = nums[i];
                        writePointer++;
                    }
                }

                while (writePointer < nums.Length)
                {
                    nums[writePointer] = 0;
                    writePointer++;
                }
            }

        }

        public static int PivotIndex(int[] nums) {
            if (nums.Length == 0) return -1;
            for (int i = 1; i < nums.Length; i++)
            {
                nums[i] += nums[i - 1];
            }

            for (int i = 0; i < nums.Length; i++)
            {
                int sumleft = (i == 0) ? 0 : nums[i - 1];
                int sumright = nums[nums.Length - 1] - nums[i];
                if(sumleft == sumright) return i;

            }
            return -1;
            
        }
    }
}