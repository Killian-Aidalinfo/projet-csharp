namespace ProjetCsharp.Models;

/// <summary>
/// Une catégorie sert à classer les tâches (ex : Travail, Perso).
/// La couleur est un code hexadécimal (ex : "#FF0000").
/// </summary>
public record Category(
    int Id,
    string Name,
    string Color = "#808080");
