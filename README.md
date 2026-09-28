# Calculatrice — Activité 4

Application Android native en C# et XAML, réalisée avec .NET MAUI Single Project.
Interface monochrome, sans compte, sans réseau et sans collecte de données.

## Fonctions

- Addition, soustraction, multiplication et division.
- Décimaux avec virgule, remise à zéro AC, effacement du dernier caractère et signe ±.
- Pourcentages : `25 % = 0,25`, `200 + 10 % = 220`, `200 × 10 % = 20`.
- Opération au-dessus du résultat et sélection visible de l'opérateur.
- Division par zéro et dépassement numérique signalés sans quitter l'application.
- Calcul immédiat de gauche à droite : `2 + 3 × 4 = 20`, comme une calculatrice standard.
- Saisie limitée à 16 chiffres, calcul en `decimal` ; un deuxième séparateur est ignoré.
- Après une erreur, une nouvelle saisie ou AC réinitialise le calcul ; les opérateurs sont désactivés.
- La rotation conserve le calcul et réorganise l'écran. Le défilement protège les petits écrans.

## Ouvrir et compiler

Prérequis : SDK .NET **10.0.103** (ou correctif compatible), charge `maui-android`, JDK 21,
SDK Android API 36 et build-tools correspondants. Le SDK est fixé dans `global.json`.

```powershell
dotnet workload install maui-android
dotnet build src/Calculatrice/Calculatrice.csproj -c Debug
```

Si les SDK Android/Java ne sont pas détectés, préciser leurs dossiers :

```powershell
dotnet build src/Calculatrice/Calculatrice.csproj -c Debug `
  -p:AndroidSdkDirectory="CHEMIN_DU_SDK_ANDROID" `
  -p:JavaSdkDirectory="CHEMIN_DU_JDK_21"
```

Ouvrir le dossier dans VS Code avec les extensions C# et .NET MAUI, ou utiliser un Visual
Studio prenant en charge .NET 10. La compilation peut aussi se faire en ligne de commande.

La cible livrée est Android. Aucun support iOS, macOS ou Windows n'est déclaré ni testé.

## Exécuter les tests

```powershell
dotnet run --project tests/Calculatrice.Tests -c Release
```

Le programme de tests échoue avec un code non nul à la première assertion incorrecte.
Il teste les comportements du moteur sans dépendance à MAUI ni à un framework de tests.

## Installer sur un émulateur ou un téléphone Android

```powershell
adb install -r CHEMIN_VERS_APK
```

L'APK de démonstration utilise une signature de développement. Il convient aux essais
locaux ; il ne s'agit pas d'une publication sur un magasin d'applications.

## Organisation

| Élément | Responsabilité |
| --- | --- |
| `src/Calculatrice/MainPage.xaml` | Disposition et contrôles natifs |
| `src/Calculatrice/MainPage.xaml.cs` | Événements, affichage, adaptation portrait/paysage |
| `src/Calculatrice/Resources/Styles/Styles.xaml` | Palette monochrome et états des touches |
| `src/Calculatrice.Core/Calculator.cs` | Saisie, opérations, pourcentages et erreurs |
| `tests/Calculatrice.Tests/Program.cs` | Cas comportementaux et limites |

Les cinq conteneurs employés sur la même page sont : **Grid** pour aligner les touches et
réorganiser les zones, **VerticalStackLayout** pour l'affichage, **HorizontalStackLayout**
pour l'en-tête, **ScrollView** pour les écrans courts, **Border** pour délimiter le résultat.
Le code-behind relie bien les événements `Clicked` au moteur, conformément à l'exercice.

## Sources

- Énoncé officiel : « Activité Numéro 4 - Atelier DevMobile.pdf », Nelson Dada, 26/09/2026.
- [Documentation .NET MAUI](https://learn.microsoft.com/dotnet/maui/).
- [Layouts .NET MAUI](https://learn.microsoft.com/dotnet/maui/user-interface/layouts/).
- [Installation .NET MAUI](https://learn.microsoft.com/dotnet/maui/get-started/installation).
- [Tutoriel partagé : .NET MAUI Development in VS Code](https://www.youtube.com/watch?v=1t2zzoW4D98).
