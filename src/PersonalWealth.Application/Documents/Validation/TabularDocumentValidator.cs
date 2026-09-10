using System.Globalization;
using PersonalWealth.Application.Documents.Tabular;

namespace PersonalWealth.Application.Documents.Validation;

public sealed class TabularDocumentValidator
{
    public IReadOnlyCollection<DocumentValidationError> Validate(
        string templateId,
        int version,
        TabularTemplate template,
        IReadOnlyList<IReadOnlyDictionary<string, string?>> rows)
    {
        var errors = new List<DocumentValidationError>();

        if (!string.Equals(template.TemplateId, templateId, StringComparison.OrdinalIgnoreCase) || template.Version != version)
        {
            errors.Add(new("UNSUPPORTED_TEMPLATE_VERSION", "The supplied template version is not supported."));
            return errors;
        }

        for (var index = 0; index < rows.Count; index++)
        {
            var rowNumber = index + 1;
            var row = rows[index];
            foreach (var field in template.Fields)
            {
                row.TryGetValue(field.SourceColumn, out var value);
                if (field.Required && string.IsNullOrWhiteSpace(value))
                {
                    errors.Add(new("REQUIRED_FIELD_MISSING", $"Required field '{field.SourceColumn}' is missing.", rowNumber, field.TargetField));
                    continue;
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                if (field.Type == TabularFieldType.Date && !DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    errors.Add(new("INVALID_DATE", $"Field '{field.SourceColumn}' is not a valid date.", rowNumber, field.TargetField));
                }
                else if (field.Type == TabularFieldType.Decimal && !decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out _))
                {
                    errors.Add(new("INVALID_NUMBER", $"Field '{field.SourceColumn}' is not a valid number.", rowNumber, field.TargetField));
                }
            }
        }

        return errors;
    }
}
