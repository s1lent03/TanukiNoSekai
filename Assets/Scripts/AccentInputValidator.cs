using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class AccentInputValidator : MonoBehaviour
{
    // Function to check for accented characters
    public static bool ContainsAccents(string inputText)
    {
        Regex regex = new Regex(@"\p{M}");
        return regex.IsMatch(inputText.Normalize(NormalizationForm.FormD));
    }
}
