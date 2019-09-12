# Extensions
.Net framework types extensions


# IEnumerable Extensions
- **SelectManyRecursive** - provides the ability to get the flatten structure from a hierarchy of objects by using provided child selector

- **FirstOrDefaultRecursive** - provides the ability to find the first object that matches certain criteria by recursively traversing a hierarchy of objects using provided child selector

- **GroupBy** - provides the ability to group a collection using provided list of properties names(as string) to group by. First group will be created by using the first property provided, then if second property is available, elements of the first group will be grouped by the second property and so on. This grouping is handy to prepare data for an UI that intend to present data in a tree structure. 

# IQueryable Extensions

- **Paginated** - apply the pagination to an IQueryable source by using provided pagination information(current page and page size)

- **FilterBy** - apply the filtering to an IQueryable by using provided filters <br/> 
A filter consists of: <br/> 
1. Property to filter on <br/> 
2. The operator to apply <br/> 
3. Value to filter by <br/> 4) The logical connection with the next filter<br/><br/> 
**Main features:**<br/>
1) Implicit type conversion (ex. target property is of type string and the provided value to filter by is an integer)<br/> 
Supported conversions:<br/> 
&nbsp;&nbsp; double, string to decimal <br/> 
&nbsp;&nbsp; decimal, string to double <br/> 
&nbsp;&nbsp; double, decimal, string to int <br/> 
&nbsp;&nbsp; decimal, double, int, DateTime to string <br/> 
&nbsp;&nbsp; string to DateTime <br/> 
&nbsp;&nbsp; string to Guid <br/>
2) Check that provided property to filter on is present on the target type <br/> 
3) Filter scope - which allow to scope filters like it would be done by using parentheses when writing linq. Scoping can be done as narrow as its needed
