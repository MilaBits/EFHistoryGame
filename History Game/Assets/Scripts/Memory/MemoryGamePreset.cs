using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using EasyButtons;
using Timeline;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace Memory
{
	[CreateAssetMenu(fileName = "New Memory Preset", menuName = "History Game/Memory Game Preset", order = 0)]
	public class MemoryGamePreset : ScriptableObject
	{
		public string description;

		[SerializeField]
		public int wrongCount = 3;

		[SerializeField, Range(2, 6)]
		private int numberOfMatches = 6;

		[SerializeField]
		public List<MatchData> matches;

#if UNITY_EDITOR

		[Header("Import CSV")]
		[SerializeField]
		private string sourceCsv = default;
		[SerializeField]
		private string targetImageAssetFolder = default;

		[Button(ButtonMode.DisabledInPlayMode)]
		private void ImportCsv()
		{
			using (var reader = new StreamReader(sourceCsv, Encoding.GetEncoding(1252)))
			using (var csv = new CsvReader(reader))
			{
				csv.Configuration.RegisterClassMap<cardImportMap>();
				csv.Configuration.Encoding = Encoding.GetEncoding(1252);
				List<CardImportData> records = csv.GetRecords<CardImportData>().ToList();

				matches.Clear();
				for (int i = 0; i < records.Count; i++)
				{
					FileInfo fromFile    = new FileInfo(records[i].ImagePath);
					string   destination = $"{targetImageAssetFolder}{fromFile.Name}";

					try
					{
						Directory.CreateDirectory(Application.dataPath.TrimEnd("Assets".ToCharArray()) +
												  targetImageAssetFolder);
						FileUtil.CopyFileOrDirectory(fromFile.FullName, destination);
					}
					catch (IOException)
					{
						Debug.Log(
							$"{destination} already exists and was skipped. If you wish to replace it, delete the current file or replace it manually.");
					}
				}

				AssetDatabase.Refresh();

				foreach (CardImportData record in records)
				{
					FileInfo fromFile    = new FileInfo(record.ImagePath);
					string   destination = $"{targetImageAssetFolder}{fromFile.Name}";
					matches.Add(new MatchData
						{
							cardDataA = new CardData
							{
								name        = record.Name,
								value       = record.Value,
								textOrImage = TextOrImage.Value,
								sprite      = AssetDatabase.LoadAssetAtPath<Sprite>(destination)
							},
							cardDataB = new CardData
							{
								name        = record.Name,
								value       = record.Value,
								textOrImage = TextOrImage.NameAndImage,
								sprite      = AssetDatabase.LoadAssetAtPath<Sprite>(destination)
							}
						}
					);
				}

				EditorUtility.SetDirty(this);
			}
		}

#endif

		public List<MatchData> GetMatches()
		{
			if (matches.Count <= 6) return matches;

			Random random = new Random();

			MatchData[] returnMatches = matches.ToArray();
			random.Shuffle(returnMatches);

			return returnMatches.ToList().GetRange(0, numberOfMatches);
		}
	}
}