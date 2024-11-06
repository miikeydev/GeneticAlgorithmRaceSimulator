using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeightManipulation
{
    private string directoryPath;

    // Constructeur pour d�finir le dossier de sauvegarde
    public WeightManipulation(string folderName)
    {
        // D�finir le chemin du dossier dans les donn�es persistantes de l'application
        directoryPath = Path.Combine(Application.persistentDataPath, folderName);

        // Cr�e le dossier s'il n'existe pas d�j�
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    // M�thode pour sauvegarder les meilleurs poids avec le nom de la g�n�ration
    public void SaveWeights(float[] bestWeights, int generationNumber)
    {
        // Cr�e un nom de fichier bas� sur le num�ro de la g�n�ration
        string fileName = $"meilleur_modele_generation{generationNumber}.txt";
        string filePath = Path.Combine(directoryPath, fileName);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Convertir les poids en une ligne de texte
            string line = string.Join(",", bestWeights);
            writer.WriteLine(line);
        }

        Debug.Log($"Les poids ont �t� sauvegard�s dans {filePath}");
    }

    // M�thode pour charger les poids d'un fichier sp�cifique
    public float[] LoadWeights(int generationNumber)
    {
        // Cr�e le nom de fichier bas� sur le num�ro de la g�n�ration
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
                    Debug.Log($"Les poids ont �t� charg�s depuis {filePath}");
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
