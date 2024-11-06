using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeightManipulation
{
    private string directoryPath;

    // Constructeur pour définir le dossier de sauvegarde
    public WeightManipulation(string folderName)
    {
        // Définir le chemin du dossier dans les données persistantes de l'application
        directoryPath = Path.Combine(Application.persistentDataPath, folderName);

        // Crée le dossier s'il n'existe pas déjà
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    // Méthode pour sauvegarder les meilleurs poids avec le nom de la génération
    public void SaveWeights(float[] bestWeights, int generationNumber)
    {
        // Crée un nom de fichier basé sur le numéro de la génération
        string fileName = $"meilleur_modele_generation{generationNumber}.txt";
        string filePath = Path.Combine(directoryPath, fileName);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Convertir les poids en une ligne de texte
            string line = string.Join(",", bestWeights);
            writer.WriteLine(line);
        }

        Debug.Log($"Les poids ont été sauvegardés dans {filePath}");
    }

    // Méthode pour charger les poids d'un fichier spécifique
    public float[] LoadWeights(int generationNumber)
    {
        // Crée le nom de fichier basé sur le numéro de la génération
        string fileName = $"meilleur_modele_generation{generationNumber}.txt";
        string filePath = Path.Combine(directoryPath, fileName);

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line = reader.ReadLine();
                if (line != null)
                {
                    // Convertir la ligne de texte en tableau de float
                    string[] weightStrings = line.Split(',');
                    float[] weights = Array.ConvertAll(weightStrings, float.Parse);
                    Debug.Log($"Les poids ont été chargés depuis {filePath}");
                    return weights;
                }
            }
        }
        else
        {
            Debug.LogWarning($"Le fichier {filePath} n'existe pas.");
        }

        return null;
    }
}
