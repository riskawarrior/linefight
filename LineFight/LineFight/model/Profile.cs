using System;
using System.Windows.Media.Imaging;

/// <summary>
/// A profil adatokat tartalmazó osztály.
/// </summary>
public class Profile {

	/// <summary>
	/// Avatar BitmapImage-t lekérõ property. Beállításnál beállítja az AvatarBytes -ot,
	/// lekérésnél az AvatarImg-t adja vissza (ha null akkor deszerializálja).
	/// </summary>
	public BitmapImage Avatar
	{
		//read property
		get;
		//write property
		set;
	}
	/// <summary>
	/// Avatar képet alkotó byteok.
	/// </summary>
	private byte[] AvatarBytes;
	/// <summary>
	/// A képet BitmapImage formátumban tárolja
	/// NonSerialized
	/// </summary>
	[NonSerialized()]
	private BitmapImage AvatarImg;
	/// <summary>
	/// Serializable
	/// </summary>
	public string Username{
		//read property
		get;
		//write property
		set;
	}

	public Profile(){

	}

	/// <summary>
	/// Inicializálja a profilt.
	/// </summary>
	/// <param name="username"></param>
	/// <param name="avatar"></param>
	public Profile(string username, BitmapImage avatar){

	}

}//end Profile