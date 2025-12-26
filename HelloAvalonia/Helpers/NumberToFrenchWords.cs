using System;

namespace AroniumFactures.Helpers;

public static class NumberToFrenchWords
{
    private static readonly string[] Units = 
    { 
        "", "un", "deux", "trois", "quatre", "cinq", "six", "sept", "huit", "neuf", 
        "dix", "onze", "douze", "treize", "quatorze", "quinze", "seize", 
        "dix-sept", "dix-huit", "dix-neuf" 
    };
    
    private static readonly string[] Tens = 
    { 
        "", "", "vingt", "trente", "quarante", "cinquante", "soixante", 
        "soixante", "quatre-vingt", "quatre-vingt" 
    };

    public static string ConvertToWords(decimal amount)
    {
        if (amount == 0) return "zéro dirham";

        var integerPart = (int)Math.Floor(amount);
        var decimalPart = (int)Math.Round((amount - integerPart) * 100);

        var words = ConvertIntegerToWords(integerPart).ToUpper();
        
        if (integerPart <= 1)
            words += " DIRHAM";
        else
            words += " DIRHAMS";

        if (decimalPart > 0)
        {
            words += $", {decimalPart:D2} CENTIMES";
        }

        return words;
    }

    private static string ConvertIntegerToWords(int number)
    {
        if (number == 0) return "";
        if (number < 20) return Units[number];

        if (number < 100)
        {
            var tens = number / 10;
            var units = number % 10;

            if (tens == 7 || tens == 9)
            {
                return Tens[tens] + (units == 0 && tens == 8 ? "s" : "-") + 
                       (tens == 7 ? Units[10 + units] : tens == 9 ? Units[10 + units] : Units[units]);
            }

            if (units == 1 && tens > 1 && tens != 8)
                return Tens[tens] + " et " + Units[units];
            
            if (tens == 8 && units == 0)
                return Tens[tens] + "s";

            return units == 0 ? Tens[tens] : Tens[tens] + "-" + Units[units];
        }

        if (number < 1000)
        {
            var hundreds = number / 100;
            var remainder = number % 100;
            
            var result = hundreds == 1 ? "cent" : Units[hundreds] + " cent";
            if (remainder == 0 && hundreds > 1) result += "s";
            if (remainder > 0) result += " " + ConvertIntegerToWords(remainder);
            
            return result;
        }

        if (number < 1000000)
        {
            var thousands = number / 1000;
            var remainder = number % 1000;
            
            var result = thousands == 1 ? "mille" : ConvertIntegerToWords(thousands) + " mille";
            if (remainder > 0) result += " " + ConvertIntegerToWords(remainder);
            
            return result;
        }

        var millions = number / 1000000;
        var millionRemainder = number % 1000000;
        
        var millionResult = millions == 1 ? "un million" : ConvertIntegerToWords(millions) + " millions";
        if (millionRemainder > 0) millionResult += " " + ConvertIntegerToWords(millionRemainder);
        
        return millionResult;
    }
}


