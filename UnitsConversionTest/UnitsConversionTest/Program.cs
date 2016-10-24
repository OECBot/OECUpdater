using System;
using UnitsConversionLib;

namespace UnitsConversionTest
{
	/// <summary>
	/// A class for constants
	/// </summary>
	public class Constants
	{
		public const double EARTHMASS = 5.972e24;
		public const double JUPITERMASS = 1.898e27;

	}
	class MainClass
	{
		public static void Main(string[] args)
		{
			
			int MetersCode = 1;
			int CentimetersCode = 3;
			int FeetCode = 7;
			int InchesCode = 8;
			UnitTable table = UnitTable.LengthTable;

			Unit meters = new Unit(MetersCode, 10, table);
			Unit inches = new Unit(InchesCode, 12, table);

			Console.WriteLine("Converting from 10 meters to centimeters/feet/inches:");
			Console.WriteLine(meters.Convert(CentimetersCode));
			Console.WriteLine(meters.Convert(FeetCode));
			Console.WriteLine(meters.Convert(InchesCode));

			Console.WriteLine("Converting from 12 inches to meters/centimeters/feet:");
			Console.WriteLine(inches.Convert(MetersCode));
			Console.WriteLine(inches.Convert(CentimetersCode));
			Console.WriteLine(inches.Convert(FeetCode));

			
			// initialize the unit codes
			int KilogramsCode = 1;
			int GramsCode = 2;
			int PoundsCode = 4;

			// initialize a UnitTable base on the class of the units
			UnitTable Wtable = UnitTable.WeightTable;

			// initialize the Unit classes(the constants are in Kgs by default)
			Unit EarthMasskilograms = new Unit(KilogramsCode, Constants.EARTHMASS, Wtable);
			Unit JupiterMasskilograms = new Unit(KilogramsCode, Constants.JUPITERMASS, Wtable);

			// convert and print out the converted units(the mass of Earth and Jupiter from kilograms to GRAMS)
			Unit EarthMassgrams = EarthMasskilograms.Convert(GramsCode);
			Unit JupiterMassgrams = JupiterMasskilograms.Convert(GramsCode);
			Console.WriteLine("Converting mass of Earth and Jupiter from kilograms to grams:");
			Console.WriteLine(EarthMassgrams);
			Console.WriteLine(JupiterMassgrams);

			// convert and print out the converted units(the mass of Earth and Jupiter from grams to KILOGRAMS)
			Console.WriteLine("Converting mass of Earth and Jupiter from grams to kilograms:");
			Console.WriteLine(EarthMassgrams.Convert(KilogramsCode));
			Console.WriteLine(JupiterMassgrams.Convert(KilogramsCode));

			// convert and print out the converted units(the mass of Earth and Jupiter from kilograms to POUNDS)
			Console.WriteLine("Converting mass of Earth and Jupiter from kilograms to pounds:");
			Console.WriteLine(EarthMasskilograms.Convert(PoundsCode));
			Console.WriteLine(JupiterMasskilograms.Convert(PoundsCode));
			Console.WriteLine();
			Console.WriteLine("Press Enter to exit");
			Console.Read();
		}
	}
}
