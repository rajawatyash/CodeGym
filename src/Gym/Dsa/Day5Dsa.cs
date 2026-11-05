namespace Gym.Dsa;

public static class Day5Dsa {
    public static bool IsPalindrome(string s) {

        int n = s.Length;
        int left = 0;
        int right = n-1;
        while(left < right)
        {
            if(!char.IsLetterOrDigit(s[left]))
            {
                left++; continue;
            }
            if(!char.IsLetterOrDigit(s[right]))
            {
                right--; continue;
            }

            if(char.ToLowerInvariant(s[left]) != char.ToLowerInvariant(s[right]))
            {
                return false;
            }
            left++;
            right--;
        }

        return true;
        
    }

    public static int[] SortedSquares(int[] nums) {
        int left = 0;
        int right = nums.Length - 1;
        int pos = right;
        int[] result = new int[nums.Length];

        while( left < right)
        {
            int leftSquare = nums[left] * nums[left];
            int rightSquare = nums[right] * nums[right];
            if(leftSquare > rightSquare)
            {
                result[pos--] = leftSquare;
                left++;
            }
            else
            {
                result[pos--] = rightSquare;
                right--;
            }
        }

        if(left == right)
        {
            result[right] = nums[right] * nums[right];
        }


        return nums;
    }

    public static int solve(List<int> A)
    {
        int ans = 0;
        A.Sort();
        for ( int i = 0; i < A.Count; i++)
        {
            ans += (i+1) * A[i];
        }
        return ans;
    }
}