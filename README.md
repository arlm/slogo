# slogo
A Logo compiler and/or interpreter for Windows and/or Linux in form of command line tool and/or IDE


### How to use a command line version and which command line switches it support ?

```
E:\projects\SLogo.git\SLogoC\bin\Debug>SLogoC.exe --help
SLogo Compiler (Console) version X.YZ
for Windows and Linux (Mono)

Usage:
SLogoC.exe filename.lgo [options]

Where options can be:
--width=x    Where x is a number that define canvas width
--height=y   Where y is a number that define canvas height
--output=z   Where z is a a name of bitmap filename where result will be saved
             or exe file where compiled Logo program will saved
--run=c      Where c is a code that will be executed

For example:
SLogoC.exe test.lgo --output=test.bmp "--run=cs snowflake 12"
will load and compile test.lgo, and if that pass fine and without any error,
then execute ClearScreen (cs) and call function snowflake with parameter equal to 12:

E:\projects\SLogo.git\SLogoC\bin\Debug>
```

### How to use a command line version ?

Let's we have a file named test.lgo with following content:

```
to square :length
  repeat 4 [ fd :length rt 90 ]
end

to rose
   clearscreen
   repeat 36 [ square 50 rt 10 ]
end

to main
  rose
end
```

To execute test.lgo, run main and get 200x200px result bitmap we'll use following command

```
E:\projects\SLogo.git\SLogoC\bin\Debug>SLogoC.exe test.lgo --output=test.bmp  --width=200 --height=200
Done in 233 miliseconds

E:\projects\SLogo.git\SLogoC\bin\Debug>
```

(Note that there are no code via --run switch specified. In this case a main function will be called)

A resulting image will be:

![Result image](http://download-codeplex.sec.s-msft.com/Download?ProjectName=slogo&DownloadId=926822)

### Why is this an 'Interpreter'?
An interpreter is usually something that can interpret source file, run it internally and show results inside itself. All currently known (as far as we know) Logo programs are interpreters

### Why is this a 'Compiler' ?
A compiler is usually a program that is able to convert source file to something that can be runnable without that program. In most cases this is EXE file. As far as we know there are no Logo programs that are compilers (except this one)

### I want to send compiled EXE file to someone who haven't SLogo installed. Is EXE file enough?
No. Together with EXE you should send a copy of TurrtleRuntime.dll file

### Can I save output to some other format or I'm stuck with bitmaps ?!
You can save files in JPG, PNG or GIF. Program will automatically recognize format from file extension you specify in command line

### Supported Logo Commands
```
PU or PenUp
PD or PenDown
REPEAT
MAKE
FD or Forward
BK or Back
RT or Right
LT or Left
CS or ClearScreen
PE or PenErase
Home
SETPC
SETSC
SetPenSize
Label
SetLabelHeight
HT or HideTurtle
ST or ShowTurtle
Stop
To
End
```

```
IF
ELSE
WHILE  
```


### Logical operators
```
AND
OR
NOT
```


