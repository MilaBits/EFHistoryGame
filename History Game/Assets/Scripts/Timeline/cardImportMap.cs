using CsvHelper.Configuration;

namespace Timeline
{
	public class cardImportMap : ClassMap<CardImportData>
	{
		public cardImportMap()
		{
			Map(m => m.Name).Name("name");
			Map(m => m.Value).Name("value");
			Map(m => m.ImagePath).Name("image");
		}
	}
}