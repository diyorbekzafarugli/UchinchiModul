using System;
using System.Collections.Generic;
using System.Text;

namespace Delegates;

public class BadNumberFinder
{
    public List<int> FindEvenNumber(List<int> ints)
    {
        var evenNums = new List<int>();

        foreach (var num in ints)
        {
            if(num % 2 == 0)
            {
                evenNums.Add(num);
            }
        }
        return evenNums;
    }


    public List<int> FindOddNumbers(List<int> ints)
    {
        var oddNumbers = new List<int>();

        foreach (var num in ints)
        {
            if(num % 2 != 0)
            {
                oddNumbers.Add(num);
            }
        }
        return oddNumbers;
    }

    public List<int> FindNumbers(List<int> ints)
    {
        List<int> nums = new();

        foreach (var num in ints)
        {
            if(num % 3 == 0 && num % 5 == 0)
            {
                nums.Add(num);
            }
        }
        return nums;
    }
}
