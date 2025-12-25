using System;
using System.Collections.Generic;
using System.Linq;

namespace HelloAvalonia.Helpers;

public static class DocumentTypeTranslator
{
    private static readonly Dictionary<string, string> Translations = new()
    {
        // 100 - Purchase / Achat
        { "Purchase", "Achat" },
        { "PURCHASE", "Achat" },
        
        // 120 - Stock Return / Retour de stock
        { "Stock Return", "Retour de stock" },
        { "STOCK RETURN", "Retour de stock" },
        
        // 200 - Sales / Ventes
        { "Sales", "BL/Facture" },
        { "SALES", "BL/Facture" },
        
        // 220 - Refund / Remboursement
        { "Refund", "Remboursement" },
        { "REFUND", "Remboursement" },
        
        // 230 - Proforma / Devis
        { "Proforma", "Devis" },
        { "PROFORMA", "Devis" },
        
        // 300 - Inventory Count / Inventaire
        { "Inventory Count", "Inventaire" },
        { "INVENTORY COUNT", "Inventaire" },
        
        // 400 - Loss And Damage / Perte et dommage
        { "Loss And Damage", "Perte et dommage" },
        { "LOSS AND DAMAGE", "Perte et dommage" },
        { "Loss and Damage", "Perte et dommage" },
    };

    public static string Translate(string englishName)
    {
        if (string.IsNullOrWhiteSpace(englishName))
            return englishName;
            
        // Try exact match first
        if (Translations.TryGetValue(englishName, out var translation))
            return translation;
            
        // Try case-insensitive match
        var key = Translations.Keys.FirstOrDefault(k => 
            k.Equals(englishName, StringComparison.OrdinalIgnoreCase));
            
        return key != null ? Translations[key] : englishName;
    }
}

