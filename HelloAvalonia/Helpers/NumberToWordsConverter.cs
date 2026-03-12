using System;

namespace AroniumFactures.Helpers;

public static class NumberToWordsConverter
{
    public static string ConvertToWords(decimal amount, string languageCode)
    {
        return languageCode switch
        {
            "ar" => ConvertToArabicWords(amount),
            "en" => ConvertToEnglishWords(amount), // Basic English
            _ => ConvertToFrenchWords(amount)
        };
    }

    #region French Implementation
    private static readonly string[] FrUnits = 
    { 
        "", "un", "deux", "trois", "quatre", "cinq", "six", "sept", "huit", "neuf", 
        "dix", "onze", "douze", "treize", "quatorze", "quinze", "seize", 
        "dix-sept", "dix-huit", "dix-neuf" 
    };
    
    private static readonly string[] FrTens = 
    { 
        "", "", "vingt", "trente", "quarante", "cinquante", "soixante", 
        "soixante", "quatre-vingt", "quatre-vingt" 
    };

    private static string ConvertToFrenchWords(decimal amount)
    {
        if (amount == 0) return "zéro dirham";

        var integerPart = (int)Math.Floor(amount);
        var decimalPart = (int)Math.Round((amount - integerPart) * 100);

        var words = ConvertFrenchIntegerToWords(integerPart).ToUpper();
        
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

    private static string ConvertFrenchIntegerToWords(int number)
    {
        if (number == 0) return "";
        if (number < 20) return FrUnits[number];

        if (number < 100)
        {
            var tens = number / 10;
            var units = number % 10;

            if (tens == 7 || tens == 9)
            {
                return FrTens[tens] + (units == 0 && tens == 8 ? "s" : "-") + 
                       (tens == 7 ? FrUnits[10 + units] : tens == 9 ? FrUnits[10 + units] : FrUnits[units]);
            }

            if (units == 1 && tens > 1 && tens != 8)
                return FrTens[tens] + " et " + FrUnits[units];
            
            if (tens == 8 && units == 0)
                return FrTens[tens] + "s";

            return units == 0 ? FrTens[tens] : FrTens[tens] + "-" + FrUnits[units];
        }

        if (number < 1000)
        {
            var hundreds = number / 100;
            var remainder = number % 100;
            
            var result = hundreds == 1 ? "cent" : FrUnits[hundreds] + " cent";
            if (remainder == 0 && hundreds > 1) result += "s";
            if (remainder > 0) result += " " + ConvertFrenchIntegerToWords(remainder);
            
            return result;
        }

        if (number < 1000000)
        {
            var thousands = number / 1000;
            var remainder = number % 1000;
            
            var result = thousands == 1 ? "mille" : ConvertFrenchIntegerToWords(thousands) + " mille";
            if (remainder > 0) result += " " + ConvertFrenchIntegerToWords(remainder);
            
            return result;
        }

        var millions = number / 1000000;
        var millionRemainder = number % 1000000;
        
        var millionResult = millions == 1 ? "un million" : ConvertFrenchIntegerToWords(millions) + " millions";
        if (millionRemainder > 0) millionResult += " " + ConvertFrenchIntegerToWords(millionRemainder);
        
        return millionResult;
    }
    #endregion

    #region Arabic Implementation
    private static readonly string[] ArUnits = { "", "واحد", "اثنان", "ثلاثة", "أربعة", "خمسة", "ستة", "سبعة", "ثمانية", "تسعة", "عشرة", "أحد عشر", "اثنا عشر", "ثلاثة عشر", "أربعة عشر", "خمسة عشر", "ستة عشر", "سبعة عشر", "ثمانية عشر", "تسعة عشر" };
    private static readonly string[] ArTens = { "", "", "عشرون", "ثلاثون", "أربعون", "خمسون", "ستون", "سبعون", "ثمانون", "تسعون" };
    private static readonly string[] ArHundreds = { "", "مائة", "مائتان", "ثلاثمائة", "أربعمائة", "خمسمائة", "ستمائة", "سبعمائة", "ثمانمائة", "تسعمائة" };

    private static string ConvertToArabicWords(decimal amount)
    {
        if (amount == 0) return "صفر درهم";

        var integerPart = (long)Math.Floor(amount);
        var decimalPart = (int)Math.Round((amount - (decimal)integerPart) * 100);

        var words = ConvertArabicIntegerToWords(integerPart);
        
        if (integerPart == 1) words = "درهم واحد";
        else if (integerPart == 2) words = "درهمان";
        else if (integerPart >= 3 && integerPart <= 10) words += " دراهم";
        else words += " درهم";

        if (decimalPart > 0)
        {
            words += " و " + ConvertArabicIntegerToWords(decimalPart) + " سنتيم";
        }

        return words;
    }

    private static string ConvertArabicIntegerToWords(long number)
    {
        if (number == 0) return "";
        if (number < 20) return ArUnits[number];
        if (number < 100)
        {
            var units = number % 10;
            var tens = number / 10;
            return (units > 0 ? ArUnits[units] + " و " : "") + ArTens[tens];
        }
        if (number < 1000)
        {
            var hundreds = number / 100;
            var remainder = number % 100;
            return ArHundreds[hundreds] + (remainder > 0 ? " و " + ConvertArabicIntegerToWords(remainder) : "");
        }
        if (number < 2000)
        {
            return "ألف" + (number % 1000 > 0 ? " و " + ConvertArabicIntegerToWords(number % 1000) : "");
        }
        if (number < 3000)
        {
            return "ألفين" + (number % 1000 > 0 ? " و " + ConvertArabicIntegerToWords(number % 1000) : "");
        }
        if (number < 1000000)
        {
            var thousands = number / 1000;
            var remainder = number % 1000;
            var thousandsPart = "";
            if (thousands <= 10) thousandsPart = ArUnits[thousands] + " آلاف";
            else thousandsPart = ConvertArabicIntegerToWords(thousands) + " ألف";
            
            return thousandsPart + (remainder > 0 ? " و " + ConvertArabicIntegerToWords(remainder) : "");
        }
        
        return number.ToString(); // Fallback for very large numbers
    }
    #endregion

    #region English Implementation (Basic)
    private static string ConvertToEnglishWords(decimal amount)
    {
        // Simple fallback
        return $"{amount:F2} DIRHAMS";
    }
    #endregion
}
