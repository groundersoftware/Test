Ako generovat kluc na Google Maps:

cd "j:\Users\snovak\AppData\Local\Xamarin\Mono for Android\"
keytool -list -v -keystore debug.keystore -alias androiddebugkey -storepass android -keypass android > SHA1

https://code.google.com/apis/console

Nakopirovat SHA1 do pola SHA1 a pridat: ";com.groundersoftware.iwantto"
Pregenerovat KEY
Prepisat Key v AndroidManifest.xml


