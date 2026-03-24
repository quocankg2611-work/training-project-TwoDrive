namespace TwoDrive.Services.Common;

public enum DocumentTypeEnum
{
    File,
    Folder
}

public static class DocumentTypeEnumExtensions
{
    public static string ToDocumentTypeString(this DocumentTypeEnum documentType)
    {
        return documentType switch
        {
            DocumentTypeEnum.File => "file",
            DocumentTypeEnum.Folder => "folder",
            _ => throw new ArgumentOutOfRangeException(nameof(documentType), $"Not expected document type value: {documentType}")
        };
    }
}