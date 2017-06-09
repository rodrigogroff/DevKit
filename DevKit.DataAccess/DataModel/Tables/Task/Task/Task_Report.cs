
namespace DataModel
{
    public partial class Task
	{
		public TaskReport Report(DevKitDB db, ref int count, TaskFilter filter, loaderOptionsTask options )
		{
            var results = ComposedFilters(db, ref count, filter, options);

            return new TaskReport
            {
                count = count,
                results = results
            };
        }
	}
}
