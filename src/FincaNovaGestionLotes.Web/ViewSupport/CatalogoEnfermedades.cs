namespace FincaNovaGestionLotes.Web.ViewSupport;

public static class CatalogoEnfermedades
{
    public static readonly IReadOnlyList<string> Nombres = new[]
    {
        "Roya del cafeto (Hemileia vastatrix)",
        "Ojo de gallo (Mycena citricolor)",
        "Mancha de hierro o chasparria (Cercospora coffeicola)",
        "Antracnosis (Colletotrichum spp.)",
        "Mancha mantecosa (Colletotrichum spp.)",
        "Derrite o muerte descendente (Phoma costarricensis)",
        "Mancha de Ascochyta (Ascochyta tarda)",
        "Mal de hilachas (Pellicularia koleroga)",
        "Mal rosado (Erythricium salmonicolor)",
        "Llaga macana o mal de machete (Ceratocystis fimbriata)",
        "Llaga negra (Rosellinia bunodes)",
        "Llaga estrellada (Rosellinia pepo)",
        "Pudrición de raíz por Armillaria (Armillaria mellea)",
        "Marchitez por Fusarium (Fusarium xylarioides)",
        "Mal del talluelo (Rhizoctonia solani)",
        "Quema o mancha bacteriana (Pseudomonas syringae)",
        "Crespera del café (Xylella fastidiosa)",
        "Fumagina (Capnodium spp.)",
        "Nematodos de la raíz (Meloidogyne y Pratylenchus)"
    };

    public static bool Existe(string? nombre) =>
        !string.IsNullOrWhiteSpace(nombre) &&
        Nombres.Contains(nombre.Trim(), StringComparer.OrdinalIgnoreCase);
}
