using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Extensions;
using SageFinancialAPI.Generators;
using SageFinancialAPI.Models;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SageFinancialAPI.Services
{
    public class FileService(AppDbContext context, ITransactionService transactionService, ILabelService labelService) : IFileService
    {
        private readonly Dictionary<string, string> ColumnMapping = new(StringComparer.OrdinalIgnoreCase)
        {
            {"Título", "Title" },
            {"Valor", "ValueBrl" },
            {"Data", "OccurredAt" },
            {"Tipo", "Type" },
            {"Categoria", "Label" },
        };

        public async Task<Entities.File> PostAsync(FileDto request, Guid profileId)
        {
            var newFile = new Entities.File
            {
                Name = request.Name,
                ProfileId = profileId,
            };

            byte[] fileBytes = Convert.FromBase64String(request.Content);
            string text = Encoding.UTF8.GetString(fileBytes);

            if (string.IsNullOrEmpty(text))
                throw new ApplicationException("Ocorreu um erro ao ler o arquivo.");

            var rows = text.Split("\r\n");
            var header = rows[0].Split(';');

            var mappingDict = request.Mapping
                .Where(m => ColumnMapping.ContainsKey(m.Prop))
                .ToDictionary(
                    m => m.Index,
                    m => ColumnMapping[m.Prop]
                );

            var transactions = new List<TransactionDto>();
            foreach (var row in rows.Skip(1))
            {
                var segments = row.Split(';');
                var transaction = new TransactionDto();

                foreach (var map in mappingDict)
                {
                    int index = map.Key;
                    string propertyName = map.Value;

                    if (index >= segments.Length) continue;

                    var property = typeof(TransactionDto).GetProperty(propertyName);
                    if (property == null) continue;

                    var value = segments[index - 1];

                    try
                    {
                        if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTimeOffset))
                        {
                            if (DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeValue))
                            {
                                if (property.PropertyType == typeof(DateTimeOffset))
                                {
                                    var dto = new DateTimeOffset(dateTimeValue);
                                    property.SetValue(transaction, dto.ToUniversalTime());
                                }

                                else
                                    property.SetValue(transaction, dateTimeValue);
                            }
                            else
                                throw new ApplicationException($"Data no formato inválido: {value}");
                        }
                        else if (property.PropertyType == typeof(TransactionType))
                        {
                            if (value.Equals("Receita", StringComparison.CurrentCultureIgnoreCase))
                                property.SetValue(transaction, TransactionType.INCOME);
                            else if (value.Equals("Despesa", StringComparison.CurrentCultureIgnoreCase))
                                property.SetValue(transaction, TransactionType.EXPENSE);
                            else
                                throw new ApplicationException($"Tipo deve ser 'Receita' ou 'Despesa'");
                        }
                        else if (property.PropertyType == typeof(LabelDto))
                        {
                            var label = await labelService.GetByTitleAsync(value);
                            if (label is not null)
                                property.SetValue(transaction, label.ToDto());
                        }
                        else
                        {
                            object convertedValue = Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture);
                            property.SetValue(transaction, convertedValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException($"Erro ao mapear a coluna {index} ({propertyName}): '{value}'. Erro: {ex.Message}");
                    }
                }
                transactions.Add(transaction);
            }

            await transactionService.PostManyAsync(transactions, profileId);
            context.Files.Add(newFile);
            await context.SaveChangesAsync();
            return newFile;
        }
    }
}