using System.Linq.Expressions;
using TwoDrive.Persistence.Models;

namespace TwoDrive.Persistence.Repositories
{
    internal static class RepositoryUtils
    {
        public static Expression<Func<FilePersistence, bool>>? BuildFilesByPathExpression(IEnumerable<string> folderPaths)
        {
            if (!folderPaths.Any()) return null;

            // Build OR predicate: file.Path.StartsWith(path1) || file.Path.StartsWith(path2) || ... for each paths in folderPaths
            var parameter = Expression.Parameter(typeof(FilePersistence), "file");
            var pathProperty = Expression.Property(parameter, nameof(FilePersistence.Path));
            var startsWithMethod = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!;

            var predicates = folderPaths
                .Select(path => (Expression)Expression.Call(
                    pathProperty,
                    startsWithMethod,
                    Expression.Constant(path)));

            var body = predicates.Aggregate(Expression.OrElse);
            var lambda = Expression.Lambda<Func<FilePersistence, bool>>(body, parameter);
            return lambda;
        }

        public static Expression<Func<FolderPersistence, bool>>? BuildFolderByPathExpression(IEnumerable<string> folderPaths)
        {
            if (!folderPaths.Any()) return null;

            // Build OR predicate: folder.Path.StartsWith(path1) || folder.Path.StartsWith(path2) || ... for each paths in folderPaths
            var parameter = Expression.Parameter(typeof(FolderPersistence), "folder");
            var pathProperty = Expression.Property(parameter, nameof(FolderPersistence.Path));

            var startsWithMethod = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!;
            var predicates = folderPaths
                .Select(path => (Expression)Expression.Call(
                    pathProperty,
                    startsWithMethod,
                    Expression.Constant(path)));
            var body = predicates.Aggregate(Expression.OrElse);
            var lambda = Expression.Lambda<Func<FolderPersistence, bool>>(body, parameter);
            return lambda;
        }
    }
}
