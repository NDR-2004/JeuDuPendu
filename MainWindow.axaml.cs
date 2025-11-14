using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;

namespace JeuDuPenduGraphique;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Image de départ
        ChangerImagePendu("pendu0.png");
        motAleatoire = MotAleatoire();
        InitialiserMotA_Trou();
    }

    private string motAleatoire;

    //longeur mot inférieure à 10 (que 9 chances)
    List<string> motA_Trouver = new List<string>()
    {"Abre", "Besion", "Coucou", "Dinde", "Echelle", "Fuseau", "Good", "Héro", "Igloo", "Jeux", "Kiwi", "Lune", "Monstre", 
    "Nez", "Ours", "Pompier", "Quotient", "Reine", "Salade", "Train", "Urler", "Voiture", "Wagon", "Xylophone", "Yaourt", "Zébre"};

    string [] fichierImage = new string[] {
        "pendu1.png", "pendu2.png", "pendu3.png", "pendu4.png", "pendu5.png"
    , "pendu6.png", "pendu7.png", "pendu8.png", "pendu9.png", "pendu10.png"
    };

    private List<string> sauvegardeLettreTaper = new List<string>()
    {};

    private List<char> motA_trou = new List<char>(){}; //mot a trou a trouver et modifiable [["C], ["_"]...]
    
    private Random rnd = new Random();

    private int incrementeImage=0;

    private void ChangerImagePendu(string nomFichier)
    {
        // Exemple : nomFichier = "pendu0.png", "pendu1.png", etc.
        var uri = new Uri($"avares://JeuDuPenduGraphique/assets/{nomFichier}");
        using var stream = AssetLoader.Open(uri);
        PenduImage.Source = new Bitmap(stream);
    }

    public string MotAleatoire()
    {
        int index = rnd.Next(motA_Trouver.Count); //genere un index entre 0 et motA_Trouver -1
        var motChoisi = motA_Trouver[index];
        return motChoisi.ToUpper();
    }

    private void InitialiserMotA_Trou()
    {   
        motA_trou.Clear();
        AfficherMot.Text = ""; // réinitialiser l'affichage
        for (int i=0; i<motAleatoire.Length; i++)
        {
            if (i==0)
            {
                motA_trou.Add(motAleatoire[0]); //ajouter 1er lettre de la liste a la liste motA_trou
            }
            else
            {
                motA_trou.Add('_');
            }
        }

        for (int i=0; i<motA_trou.Count; i++)
        {
            AfficherMot.Text += motA_trou[i]+" ";
        }
    }

    private void BtnLettre_click(object? sender, RoutedEventArgs e)
    {
        var button = (Button)sender!;
        button.IsEnabled = false;   //on désactive le bouton cliqué

        var texte = button.Content?.ToString().Trim();
        if (string.IsNullOrEmpty(texte))
        {
            return;
        }

        char valeur = char.ToUpper(texte[0]);
        
        if (sauvegardeLettreTaper.Contains(valeur.ToString()))
        {
            return;
        }

        ZoneAffichage.Text += valeur.ToString()+" "; //en ajoute la valeur déjà tapper
        sauvegardeLettreTaper.Add(valeur.ToString());

        if (motAleatoire.Contains(valeur))
        {
            for (int i=1; i<motA_trou.Count; i++)
            {
                if (motAleatoire[i] == valeur)
                {
                    motA_trou[i] = valeur;
                }
            }

            // mise à jour de l'affichage
            AfficherMot.Text = "";
            for (int i=0; i<motA_trou.Count; i++)
            {
                AfficherMot.Text += motA_trou[i] + " ";
            }

            // 🔹 ICI : test de victoire
            var motCourant = new string(motA_trou.ToArray());
            if (motCourant == motAleatoire)
            {
                SetClavierEnabled(false);
                AfficherMot.Text = $"Vous avez gagné, le mot\nest: {motAleatoire}";
                return;
            }
        }
        else
        {
            if (incrementeImage < fichierImage.Length - 1)
            {
                ChangerImagePendu(fichierImage[incrementeImage]);
                incrementeImage += 1;
            }
            else
            {
                incrementeImage = 9;
                ChangerImagePendu(fichierImage[incrementeImage]);
                SetClavierEnabled(false);
                AfficherMot.Text = $"Vous avez perdu le mot\nétait: {motAleatoire}";
            }
        }
    }

    private void SetClavierEnabled(bool enabled)
    {
        foreach (var child in ClavierPanel.Children)
        {
            if (child is Button b)
            {
                // on laisse le bouton "Rejouer" toujours actif
                if (b.Content?.ToString() == "🔄")
                    continue;

                b.IsEnabled = enabled;
            }
        }
    }


    private void BtnRejouer_click(object? sender, RoutedEventArgs e)
    {
        // ici plus tard : reset du jeu, remettre pendu0.png, réactiver les boutons, etc.
        motA_trou.Clear();
        sauvegardeLettreTaper.Clear();
        AfficherMot.Text = "";
        ZoneAffichage.Text = "";
        incrementeImage = 0;

        ChangerImagePendu("pendu0.png");
        motAleatoire = MotAleatoire();
        InitialiserMotA_Trou();
        SetClavierEnabled(true);   // on réactive toutes les lettres

    }
}
