using System.Globalization;

namespace Calculatrice.Core;

/// <summary>Immediate-execution calculator. No UI, storage or network dependencies.</summary>
public sealed class Calculator
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
    private decimal? accumulator;
    private string? operation;
    private string completedExpression = "";
    private bool startNewEntry;
    private bool hasOperand;

    public string Entry { get; private set; } = "0";
    public string Display => Entry.Replace('.', ',');
    public string Message { get; private set; } = "";
    public bool IsError { get; private set; }
    public string? PendingOperation => operation;
    public string Expression => operation is not null
        ? $"{Format(accumulator!.Value).Replace('.', ',')} {operation}"
        : completedExpression;

    public void Press(string key)
    {
        Message = IsError ? Message : "";
        if (key.Length == 1 && key[0] is >= '0' and <= '9')
            EnterDigit(key);
        else if (key is "," or ".")
            EnterDecimal();
        else if (key is "+" or "−" or "×" or "÷")
            SelectOperation(key);
        else if (key == "=")
            EqualsResult();
        else if (key == "AC")
            Clear();
        else if (key == "⌫")
            Backspace();
        else if (key == "±")
            ChangeSign();
        else if (key == "%")
            Percent();
        else
            throw new ArgumentException("Touche inconnue.", nameof(key));
    }

    private void PrepareEntry()
    {
        if (IsError) Clear();
        if (startNewEntry) Entry = "0";
        startNewEntry = false;
        completedExpression = "";
        hasOperand = true;
    }

    private void EnterDigit(string digit)
    {
        PrepareEntry();
        if (Entry.Count(char.IsDigit) >= 16)
        {
            Message = "Saisie limitée à 16 chiffres.";
            return;
        }
        Entry = Entry switch
        {
            "0" => digit,
            "-0" => "-" + digit,
            _ => Entry + digit
        };
    }

    private void EnterDecimal()
    {
        PrepareEntry();
        if (!Entry.Contains('.')) Entry += ".";
    }

    private void SelectOperation(string next)
    {
        if (IsError) return;
        if (operation is not null && hasOperand && !Calculate()) return;
        accumulator = Value();
        operation = next;
        completedExpression = "";
        startNewEntry = true;
        hasOperand = false;
    }

    private void EqualsResult()
    {
        if (IsError || operation is null || !hasOperand) return;
        string expression = $"{Expression} {Display} =";
        if (!Calculate()) return;
        completedExpression = expression;
        accumulator = null;
        operation = null;
        startNewEntry = true;
        hasOperand = false;
    }

    private bool Calculate()
    {
        decimal right = Value();
        if (operation == "÷" && right == 0)
        {
            Fail("Division par zéro impossible. Saisissez un nombre ou appuyez sur AC.");
            return false;
        }
        try
        {
            decimal result = operation switch
            {
                "+" => accumulator!.Value + right,
                "−" => accumulator!.Value - right,
                "×" => accumulator!.Value * right,
                "÷" => accumulator!.Value / right,
                _ => throw new InvalidOperationException("Opération manquante.")
            };
            Entry = Format(result);
            return true;
        }
        catch (OverflowException)
        {
            Fail("Résultat trop grand. Saisissez un nombre ou appuyez sur AC.");
            return false;
        }
    }

    private void ChangeSign()
    {
        if (IsError) return;
        if (startNewEntry && operation is not null) Entry = "0";
        startNewEntry = false;
        completedExpression = "";
        hasOperand = true;
        Entry = Entry.StartsWith('-') ? Entry[1..] : "-" + Entry;
    }

    private void Percent()
    {
        if (IsError || (startNewEntry && operation is not null)) return;
        try
        {
            decimal fraction = Value() / 100m;
            // Standard calculator convention: 200 + 10% = 220; 200 × 10% = 20.
            if (operation is "+" or "−") fraction *= accumulator!.Value;
            Entry = Format(fraction);
            completedExpression = "";
            startNewEntry = false;
            hasOperand = true;
        }
        catch (OverflowException)
        {
            Fail("Pourcentage trop grand. Saisissez un nombre ou appuyez sur AC.");
        }
    }

    private void Backspace()
    {
        if (IsError) { Clear(); return; }
        if (startNewEntry && operation is not null) return;
        completedExpression = "";
        startNewEntry = false;
        hasOperand = true;
        Entry = Entry.Length > 1 ? Entry[..^1] : "0";
        if (Entry == "-") Entry = "0";
    }

    private void Fail(string message)
    {
        IsError = true;
        Message = message;
    }

    public void Clear()
    {
        Entry = "0";
        accumulator = null;
        operation = null;
        completedExpression = "";
        startNewEntry = false;
        hasOperand = false;
        IsError = false;
        Message = "";
    }

    private decimal Value() => decimal.Parse(Entry, NumberStyles.Number, Invariant);
    private static string Format(decimal value) => value.ToString("0.############################", Invariant);
}
