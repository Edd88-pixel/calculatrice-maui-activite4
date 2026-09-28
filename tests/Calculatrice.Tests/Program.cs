using Calculatrice.Core;

var cases = new (string Name, string Keys, string Expected)[]
{
    ("Addition", "1 2 + 3 =", "15"),
    ("Soustraction négative", "2 − 5 =", "-3"),
    ("Multiplication", "7 × 8 =", "56"),
    ("Division", "9 ÷ 4 =", "2.25"),
    ("Décimaux exacts", "0 , 1 + 0 , 2 =", "0.3"),
    ("Point accepté", "1 . 5 × 2 =", "3"),
    ("Séparateur initial", ", 5 + , 2 5 =", "0.75"),
    ("Double séparateur ignoré", "1 , 2 , 3", "1.23"),
    ("Zéros initiaux", "0 0 0 5", "5"),
    ("AC efface tout", "9 + 2 AC 4 =", "4"),
    ("Retour caractère", "1 2 3 ⌫", "12"),
    ("Retour séparateur", "1 , ⌫", "1"),
    ("Retour dernier chiffre", "9 ⌫ ⌫", "0"),
    ("Retour négatif", "5 ± ⌫", "0"),
    ("Changement signe", "5 ± + 2 =", "-3"),
    ("Deux changements signe", "5 ± ±", "5"),
    ("Signe avant nombre", "± 3 + 2 =", "-1"),
    ("Second opérande négatif", "8 + ± 2 =", "6"),
    ("Pourcentage seul", "2 5 %", "0.25"),
    ("Pourcentage addition", "2 0 0 + 1 0 % =", "220"),
    ("Pourcentage soustraction", "2 0 0 − 1 0 % =", "180"),
    ("Pourcentage multiplication", "2 0 0 × 1 0 % =", "20"),
    ("Pourcentage division", "2 0 0 ÷ 1 0 % =", "2000"),
    ("Opérateur remplacé", "9 + × 2 =", "18"),
    ("Calcul immédiat gauche à droite", "2 + 3 × 4 =", "20"),
    ("Résultat réutilisé", "2 + 3 = × 4 =", "20"),
    ("Nouveau calcul", "2 + 3 = 7", "7"),
    ("Décimal après résultat", "2 + 3 = , 5", "0.5"),
    ("Égal sans opérande", "8 + =", "8"),
    ("Égal répété sans effet", "8 + 2 = =", "10"),
    ("Pourcentage sans opérande", "8 + % 2 =", "10"),
    ("Retour sans opérande", "8 + ⌫ 2 =", "10"),
    ("Signe du résultat", "8 + 2 = ±", "-10"),
    ("Correction résultat", "8 + 2 = ⌫", "1"),
    ("Seize chiffres", "1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7", "1234567890123456"),
    ("Récupération zéro par chiffre", "8 ÷ 0 = 3 + 2 =", "5"),
    ("Récupération zéro par AC", "8 ÷ 0 = AC 4", "4"),
    ("Récupération zéro par retour", "8 ÷ 0 = ⌫", "0"),
    ("Récupération zéro par décimal", "8 ÷ 0 = , 5", "0.5"),
    ("Zéro numérateur", "0 ÷ 5 =", "0"),
    ("Négatifs décimaux", "± , 5 × 2 =", "-1")
};
int assertions = 0;
foreach (var test in cases)
{
    Calculator calculator = Run(test.Keys);
    Check(calculator.Entry == test.Expected && !calculator.IsError, test.Name);
}

Calculator error = Run("8 ÷ 0 =");
Check(error.IsError && error.Message.Contains("zéro"), "Division zéro explicite");
error.Press("+");
Check(error.IsError, "Opérateur ne masque pas erreur");
Check(Run("0 ÷ 0 =").IsError, "Zéro divisé par zéro");
Check(Run("9 ÷ 0 +").IsError, "Erreur pendant calcul enchaîné");
Check(Run("1 2 +").Expression == "12 +", "Opération visible");
Check(Run("1 , 5 + 2 =").Expression == "1,5 + 2 =", "Expression résultat");
Check(Run("1 , 5").Display == "1,5", "Affichage français");
Check(Run("1 + AC").Expression == "", "AC efface expression");
Check(Run("9 9 9 9 9 9 9 9 9 9 9 9 9 9 9 9 × 9 9 9 9 9 9 9 9 9 9 9 9 9 9 9 9 =").IsError,
    "Dépassement decimal explicite");
Check(Run("1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7").Message.Contains("16"), "Limite expliquée");
try { new Calculator().Press("?"); throw new Exception("Touche invalide acceptée"); }
catch (ArgumentException) { assertions++; }

Console.WriteLine($"PASS — {assertions} assertions comportementales.");

Calculator Run(string keys)
{
    var calculator = new Calculator();
    foreach (string key in keys.Split(' ')) calculator.Press(key);
    return calculator;
}

void Check(bool condition, string name)
{
    if (!condition) throw new InvalidOperationException($"ÉCHEC : {name}");
    assertions++;
    Console.WriteLine($"PASS {name}");
}
