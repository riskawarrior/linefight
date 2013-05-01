using System;
using System.Windows.Media.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace UniversalLobby.model {
	/// <summary>
	/// A profil adatokat tartalmaz� oszt�ly.
	/// </summary>
	public class Profile {

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
		/// Avatar BitmapImage-t lek�r� property. Be�ll�t�sn�l be�ll�tja az AvatarBytes -ot,
		/// lek�r�sn�l az AvatarImg-t adja vissza (ha null akkor deszerializ�lja).
		/// </summary>
		public BitmapImage Avatar {
			//read property
			get {
				if (AvatarImg == null) {
					AvatarImg = new BitmapImage();
					MemoryStream ms = new MemoryStream(AvatarBytes);
					ms.Seek(0, SeekOrigin.Begin);

					AvatarImg.BeginInit();
					AvatarImg.StreamSource = ms;
					AvatarImg.EndInit();
				}
				return AvatarImg;
			}
			//write property
			set {
				AvatarImg = value;
				MemoryStream ms = new MemoryStream();

				/*PngBitmapEncoder encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(value));
				encoder.Save(ms);
				ms.Seek(0,SeekOrigin.Begin);
				AvatarBytes = ms.ToArray();*/

			}
		}

		/// <summary>
		/// Serializable
		/// </summary>
		public string Username {
			//read property
			get;
			//write property
			set;
		}

		public Profile() {

		}

		/// <summary>
		/// Inicializ�lja a profilt.
		/// </summary>
		/// <param name="username">Felhaszn�l�n�v</param>
		/// <param name="avatar">Profilk�p</param>
		public Profile(string username, BitmapImage avatar) {
			Username = username;
			Avatar = avatar;
		}

	}
}