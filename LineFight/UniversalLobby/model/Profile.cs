using System;
using System.Windows.Media.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace UniversalLobby.model {
	/// <summary>
	/// A profil adatokat tartalmazó osztály.
	/// </summary>
	public class Profile {

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
		/// Avatar BitmapImage-t lekérõ property. Beállításnál beállítja az AvatarBytes -ot,
		/// lekérésnél az AvatarImg-t adja vissza (ha null akkor deszerializálja).
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
		/// Inicializálja a profilt.
		/// </summary>
		/// <param name="username">Felhasználónév</param>
		/// <param name="avatar">Profilkép</param>
		public Profile(string username, BitmapImage avatar) {
			Username = username;
			Avatar = avatar;
		}

	}
}