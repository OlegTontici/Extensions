using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Extensions.IEnumerable
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            foreach (var item in source)
            {
                yield return item;

                var children = selector(item);

                if (children != null)
                {
                    foreach (var child in children.SelectManyRecursive(selector))
                    {
                        yield return child;
                    }
                }
            }
        }

        public static T FirstOrDefaultRecursive<T>(this IEnumerable<T> source, Predicate<T> predicate, Func<T, IEnumerable<T>> childrenSelector)
        {
            foreach (var item in source)
            {
                bool found = predicate(item);
                if (found)
                {
                    return item;
                }
                else
                {
                    var result = childrenSelector(item).FirstOrDefaultRecursive(predicate, childrenSelector);
                    if (result == null)
                    {
                        continue;
                    }
                    return result;
                }
            }

            return default(T);
        }

        public static IEnumerable<Group<TElement>> GroupBy<TElement>(this IEnumerable<TElement> elements, IEnumerable<string> propertyNames)
        {
            if (propertyNames == null)
            {
                throw new ArgumentNullException(nameof(propertyNames));
            }

            if (!propertyNames.Any())
            {
                throw new ArgumentException($"Parameter '{nameof(elements)}' can not be empty");
            }

            if (!elements.Any())
            {
                throw new ArgumentException($"Parameter '{nameof(elements)}' can not be empty");
            }

            string propertyToGroupBy = propertyNames.First();

            var metadataOfpropertyToGroupBy = typeof(TElement).GetProperty(propertyToGroupBy);

            if (metadataOfpropertyToGroupBy == null)
            {
                throw new ArgumentException($"Property '{propertyToGroupBy}' is not present on type '{typeof(TElement).FullName}'");
            }

            var parameterExpression = Expression.Parameter(typeof(TElement));
            var propertyAccessExpression = Expression.Property(parameterExpression, metadataOfpropertyToGroupBy.Name);
            var toStringExpression = Expression.Call(propertyAccessExpression, typeof(object).GetMethod("ToString"));

            var keySelector = Expression.Lambda<Func<TElement, string>>(toStringExpression, parameterExpression).Compile();

            var groupedElements = elements.GroupBy(keySelector);

            var groupingResult = new List<Group<TElement>>();

            var namesOfPropertiesLeftForGrouping = propertyNames.Skip(1);

            groupingResult.AddRange(groupedElements.Select(items =>
            {
                var newGroup = new Group<TElement>(items.Key);
                var stillNeedToGroupItems = namesOfPropertiesLeftForGrouping.Any();

                if (stillNeedToGroupItems)
                {
                    newGroup.Add(items.GroupBy(namesOfPropertiesLeftForGrouping));
                }
                else
                {
                    newGroup.Add(items);
                }

                return newGroup;
            }));

            return groupingResult;
        }

        [DebuggerDisplay("Key = {Key}")]
        public class Group<TElement>
        {
            private IEnumerable<TElement> _items;
            private IEnumerable<Group<TElement>> _nestedGroups;

            public string Key { get; }
            public bool IsLeaf => _items.Any() && !_nestedGroups.Any();
            public IReadOnlyCollection<TElement> Items => _items.ToList().AsReadOnly();
            public IReadOnlyCollection<Group<TElement>> NestedGroups => _nestedGroups.ToList().AsReadOnly();

            public Group(string key)
            {
                Key = key;
                _items = new List<TElement>();
                _nestedGroups = new List<Group<TElement>>();
            }

            public void Add(TElement item)
            {
                _items.Append(item);
            }
            public void Add(IEnumerable<TElement> items)
            {
                _items = _items.Concat(items);
            }

            public void Add(Group<TElement> group)
            {
                _nestedGroups.Append(group);
            }
            public void Add(IEnumerable<Group<TElement>> groups)
            {
                _nestedGroups = _nestedGroups.Concat(groups);
            }
        }
    }
}
