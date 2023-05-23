using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppGestCulture.Models;
public class Parcelle
{
    [PrimaryKey, AutoIncrement]
    public int Id_parc { get; set; }

    [ForeignKey(typeof(Exploitation))]
    public int Id_exploi { get; set; }

    public int Id_espece { get; set; }

    public string Code_parc { get; set; }

    public float Surface { get; set; }

    public float Rendement_prev { get; set; }

    public float Rendement_reel { get; set; }

    public string Annee { get; set; }

    public string Numero { get; set; }


    [ManyToOne]
    public Exploitation Exploitation { get; set; }
}