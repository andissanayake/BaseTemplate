using System.ComponentModel.DataAnnotations;

namespace BaseTemplate.Application.Common.Validation;
public static class ModelValidator
{
    public static (bool IsValied, Dictionary<string, string[]> Errors) Validate<T>(T model)
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

        var ret = errorDict.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());
        return (ret.Count == 0, ret);
    }
}
