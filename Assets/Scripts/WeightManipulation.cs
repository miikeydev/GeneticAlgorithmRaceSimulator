using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeightManipulation
{
    private string directoryPath; // Directory path for saving files

    // Constructor to set the save directory
    public WeightManipulation(string folderName)
    {
        // Define the path in the application's persistent data
        directoryPath = Path.Combine(Application.persistentDataPath, folderName);

        // Create the directory if it doesn't exist
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    // Method to save the best weights with the generation name
    public void SaveWeights(float[] bestWeights, int generationNumber)
    {
        // Create a filename based on the generation number
        string fileName = $"best_model_generation{generationNumber}.txt";
        string filePath = Path.Combine(directoryPath, fileName);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Convert weights to a single line of text
            string line = string.Join(",", bestWeights);
            writer.WriteLine(line);
        }

        Debug.Log($"Weights have been saved to {filePath}");
    }

    // Method to load weights from a specific file
    public float[] LoadWeights(int generationNumber)
    {
        // Create the filename based on the generation number
        string fileName = $"best_model_generation{generationNumber}.txt";
        string filePath = Path.Combine(directoryPath, fileName);

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line = reader.ReadLine();
                if (line != null)
                {
                    // Convert the text line to a float array
                    string[] weightStrings = line.Split(',');
                    float[] weights = Array.ConvertAll(weightStrings, float.Parse);
                    Debug.Log($"Weights have been loaded from {filePath}");
                    return weights;
                }
            }
        }
        else
        {
            Debug.LogWarning($"The file {filePath} does not exist.");
        }

        return null;
    }
}
