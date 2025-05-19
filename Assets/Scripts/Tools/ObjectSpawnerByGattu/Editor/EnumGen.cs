using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EnumGen
{
    public void CreateEnum(List<string> enumNames, string enumName, string scriptsFolder, bool isRefresh)
    {
        string content = EnumTemplate(enumName, enumNames);
        GenerateScript(enumName, content, scriptsFolder, isRefresh);
    }

    // Generar el script del enum
    private void GenerateScript(string scriptName, string scriptContents, string scriptsFolder, bool isRefresh)
    {
        string folderPath = Path.Combine(Application.dataPath, scriptsFolder);

        // Asegurarse de que la carpeta exista o crearla si no existe
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Crear la ruta completa del archivo del script
        string scriptPath = Path.Combine(folderPath, scriptName + ".cs");

        // Escribir el contenido del script en el archivo
        File.WriteAllText(scriptPath, scriptContents);

        // Actualizar el Asset Database para reflejar los cambios en el proyecto de Unity
        if(isRefresh) AssetDatabase.Refresh();

        // Imprimir un mensaje en la consola de Unity para confirmar la generación exitosa del script
        Debug.Log("Script generated: " + scriptPath);
    }

    // Plantilla para el enum
    private string EnumTemplate(string enumName, List<string> enumList)
    {
        string txt = $"public enum {enumName}\n{{\n";

        foreach (string enumValue in enumList)
        {
            txt += $"    {enumValue},\n";
        }

        txt += "}\n";

        return txt;
    }
}
