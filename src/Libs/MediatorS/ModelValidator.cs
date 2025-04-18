using System.ComponentModel.DataAnnotations;

namespace MediatorS;
public static class ModelValidator
{
    public static Dictionary<string, string[]> Validate(object model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, validateAllProperties: true);

        var errorDict = new Dictionary<string, List<string>>();

        foreach (var result in results)
        {
            foreach (var memberName in result.MemberNames)
            {
                if (!errorDict.ContainsKey(memberName))
                    errorDict[memberName] = [];

                errorDict[memberName].Add(result.ErrorMessage ?? "Invalid value.");
            }
        }

        return errorDict.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());
    }
}
