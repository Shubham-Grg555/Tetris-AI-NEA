# Tetris NEA

## Description
A tetris clone where you can play against 3 AI difficulties, or just do solo play,
where you need to make an account with a valid email address to play. To make an
account you need to verify your email address by typing in a verification code,
that gets emailed to you. Once the account has been verified, all necessary data
like email, password etc is stored in a firebase database, so you are able to
log in and play.

Can view examples of the project further on in the README.md

## Side note
# UI
A very basic UI was used, as the UI quality did not effect the marks, as only
the actual code and the quality of the code gave marks.

# "Missing files / content"
Did have to remove some firebase stuff for privacy (API) / too large off a file
(Firebase plugin). If for some reason you would want to clone this software,
just get the firebase package into unity and all should be fine once you replace
all the API, email sending stuff and firebase database with your own, as I have
also shut down the firebase database the project used.

## Sample images of the project in action:
# Note: Didn't show all examples of testing because that would be way too many images.

# Solo play:
![image](https://github.com/user-attachments/assets/00fa61df-905b-4de5-bb5f-154f8e7201e0)

# Versus AI:
![image](https://github.com/user-attachments/assets/d507cff5-a727-43a1-a102-a9d5bbd17797)

# Registering account and data is added to the database:
![image](https://github.com/user-attachments/assets/2a9e2649-d060-4c6f-aa30-91ebede5a7fc)

(Verification code email):
![image](https://github.com/user-attachments/assets/9d2acffd-774a-4857-b0c8-76eca7d90cd1)

(Password is securely stored using firebase authentication):
![image](https://github.com/user-attachments/assets/e5b6c555-d71d-49fd-b642-6a250a55db8c)

![image](https://github.com/user-attachments/assets/c6ab7fec-a1d7-49c6-8bd4-a98d9fcca6af)


# Logging in:
(After successful sign it, it moves you into the main menu where you can choose solo
play or AI difficulty to play against)
![image](https://github.com/user-attachments/assets/3d3fc9f1-3075-48b1-ac9d-6dc461213fab)

# Some invalid UI checks:
(Added in an extra space in the verificatino code)
![image](https://github.com/user-attachments/assets/96a92c87-ce98-474a-bca7-6149fafa38d9)

(Need unique username):
![image](https://github.com/user-attachments/assets/2e39a4a5-6342-42da-97e2-8bcbc6a1b5dc)

(Need to hit password requirements):
![image](https://github.com/user-attachments/assets/2cdd6e7d-366d-4192-8ddf-da965943a275)

(Trying to login with an email or username that does not exist):
![image](https://github.com/user-attachments/assets/989f2ef6-0313-4a18-884e-b4c54abb6892)


(Trying to login in with an incorrect password):
![image](https://github.com/user-attachments/assets/e6d43132-05fa-418d-9d7b-6f932a24be89)



## LISCENSE
MIT License

Copyright (c) [2025] [Shubham Gurung]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
