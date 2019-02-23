using System;

namespace AzureTableEventSourcingTest.Domain
{
	public class VersionTag: IEquatable<VersionTag>
	{
		public VersionTag(int value)
		{
			Value = value;
		}

		public int Value { get; }

		public override int GetHashCode() => Value.GetHashCode();

		public override bool Equals(object obj) => Equals(obj as VersionTag);

		public bool Equals(VersionTag other) => Value == other?.Value;
	}
}
