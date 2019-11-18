using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using EasyButtons;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace Timeline
{
	[CreateAssetMenu(fileName = "New Timeline Preset", menuName = "History Game/Timeline Game Preset", order = 0)]
	[Serializable]
	public class TimelineGamePreset : ScriptableObject
	{
		public string description;

		[Header("Settings")]
		[SerializeField]
		private int numberOfItems = 8;

		[Header("Data")]
		[SerializeField]
		private List<CardData> cards = new List<CardData>();

		public List<TimeRange> timeRanges = new List<TimeRange>();

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

				cards.Clear();
				for (int i = 0; i < records.Count; i++)
				{
					FileInfo fromFile    = new FileInfo(records[i].ImagePath);
					string   destination = $"{targetImageAssetFolder}{fromFile.Name}";

					try
					{
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
					FileInfo fromFile = new FileInfo(record.ImagePath);
					cards.Add(new CardData
					{
						name        = record.Name,
						value       = record.Value,
						textOrImage = TextOrImage.NameAndImage,
						sprite      = AssetDatabase.LoadAssetAtPath<Sprite>($"{targetImageAssetFolder}{fromFile.Name}")
					});
				}

				EditorUtility.SetDirty(this);
			}
		}

#endif

		public List<CardData> GetCards()
		{
			if (cards.Count <= 8) return cards;

			Random random = new Random();

			CardData[] returnCards = cards.Where(x => x.value <= timeRanges.Last().end).ToArray();

			random.Shuffle(returnCards);

			return returnCards.ToList().GetRange(0, numberOfItems);
		}
	}
}