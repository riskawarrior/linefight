using System;
using System.Windows.Media.Imaging;

/// <summary>
/// A profil adatokat tartalmaz� oszt�ly.
/// </summary>
public class Profile {

	/// <summary>
	/// Avatar BitmapImage-t lek�r� property. Be�ll�t�sn�l be�ll�tja az AvatarBytes -ot,
	/// lek�r�sn�l az AvatarImg-t adja vissza (ha null akkor deszerializ�lja).
	/// </summary>
	public BitmapImage Avatar
	{
		//read property
		get;
		//write property
		set;
	}
	/// <summary>
	/// Avatar k�pet alkot� byteok.
	/// </summary>
	private byte[] AvatarBytes;
	/// <summary>
	/// A k�pet BitmapImage form�tumban t�rolja
	/// NonSerialized
	/// </summary>
	[NonSerialized()]
	private BitmapImage AvatarImg;
	/// <summary>
	/// Serializable
	/// </summary>
	public string Username
	{
		//read property
		get;
		//write property
		set;
	}

	public Profile(){
		
	}

	/// <summary>
	/// Inicializ�lja a profilt.
	/// </summary>
	/// <param name="username">Felhaszn�l�n�v</param>
	/// <param name="avatar">Profilk�p</param>
	public Profile(string username, BitmapImage avatar){
		Username = username;
		Avatar = avatar;
	}

}//end Profile