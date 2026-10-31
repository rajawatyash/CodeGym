using System;

namespace Gym.Day02;

public class NumArray {

    private readonly int[] _nums;
    public NumArray(int[] nums) {
        for (int i = 1; i < nums.Length; i++)
        {
            nums[i] += nums[i - 1];
        }
        
        _nums = nums;
        
    }
    
    public int SumRange(int left, int right) {
        if (left == 0)
        {
            return _nums[right];
        }
        
        return _nums[right] - _nums[left - 1];
    }
}

/**
 * Your NumArray object will be instantiated and called as such:
 * NumArray obj = new NumArray(nums);
 * int param_1 = obj.SumRange(left,right);
 */
