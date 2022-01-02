# CalcC - a calculator compiler

## Purpose

This project has several purposes:

1. To get your .NET (pronounced "dot net") environment set up
1. To get you used to using `mstest` to test your compiler
1. To build a minimal compiler that targets MSIL/CIL code (the pseudo-assembly language of .NET)
1. To convince you that compilers aren't magic!

See the bottom of this document for specific instructions on the assignment.

## Compilers aren't magic!

They're just _tedious_.

What do I mean by that? The key to understanding and writing compilers is to break things down into tiny, repeatable patterns. Examples of these patterns are:

-   _keywords_, like `if` and `for`
-   _binary expressions_, like `3 + 3`
-   _statements_, like `var x = 3 * y;`

Likewise, when you're writing the output of the compiler (in our case, CIL code), you want to break it down into pieces, too, like:

-   the _preamble_, the stuff that you have to put at the top of the file, just because
-   the _postamble_, which is just the stuff at the bottom
-   _idioms_, which are just little chunks of code that translate directly to something in the source

When writing a compiler, you can expect to write a _lot_ of small classes and a bunch of long `if-else` and `switch-case` statements.

One of the things I hope you get out of this assignment is being able to break down the input into little pieces and then translate each of those pieces into a little idiom of CIL in the output.

## The calculator language

We're going to build a compiler that takes simple RPN calculator expressions and compiles them into a .NET executable.

Here is a simple example of the language:

```
3 4 +
```

The output of this program will be

```
7
```

### Operators

Your compiler must support the following operators:

-   `+` add
-   `-` subtract
-   `*` multiply
-   `/` divide
-   `%` modulus (aka remainder)
-   `sqrt` square root

(You only need to support integers; we don't need to worry about floating point numbers.)

### Registers

Additionally, the language must support 26 named registers (like variables), named `a` through `z`.

To store the top of the stack into register `c`:

```
sc
```

To push the contents of register `f` onto the stack:

```
rf
```

### Complicated example

To calculate the hypotenuse of a right triangle with perpendicular sides of length 6 and 8:

```
6 sx 8 sy rx rx * ry ry * + sqrt
```

The output of this program will be

```
10
```

## Compilation to CIL

[CIL (formerly MSIL)](https://en.wikipedia.org/wiki/Common_Intermediate_Language) is the pseudo-assembly language of .NET. You can easily see what CIL is generated for a C# program at [sharplab.io](https://sharplab.io).

One way to solve this assignment is to think about what the program would look like if you wrote it in C#. `3 4 + sx rx rx *` might look like this:

```csharp
using System;
using System.Collections.Generic;

static class Program
{
    static void Main(string[] args)
    {
        // set up our stack and registers
        var stack = new Stack<int>();
        var registers = new Dictionary<char,int>();

        // calculate the expression
        stack.Push(3);
        stack.Push(4);
        stack.Push(stack.Pop()+stack.Pop());
        registers['x'] = stack.Pop();
        stack.Push(registers['x']);
        stack.Push(registers['x']);
        stack.Push(stack.Pop()*stack.Pop());

        // print the results
        Console.WriteLine(stack.Pop());
    }
}
```

This is simple enough--use a `Stack<int>` to be the calculation stack and a `Dictionary<char,int>` to store the registers.

(If you paste that code into [sharplab.io](https://sharplab.io) to see the CIL, **do not be intimidated by the output!** Most of the output is "boilerplate".)

Here is what the equivalent CIL would be, with comments to explain what's going on:

```
// Preamble
.assembly _ { }

.method public hidebysig static void main() cil managed
{
    .entrypoint
    .maxstack 3

    // Declare two local vars: a Stack<int> and a Dictionary<char, int>
    .locals init (
        [0] class [System.Collections]System.Collections.Generic.Stack`1<int32> stack,
        [1] class [System.Private.CoreLib]System.Collections.Generic.Dictionary`2<char, int32> registers
    )

    // Initialize the Stack<>
    newobj instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::.ctor()
    stloc.0
    // Initialize the Dictionary<>
    newobj instance void class [System.Private.CoreLib]System.Collections.Generic.Dictionary`2<char, int32>::.ctor()
    stloc.1

    // Push 3 on the stack
    ldloc.0
    ldc.i4.3
    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)

    // Push 4 on the stack
    ldloc.0
    ldc.i4.4
    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)

    // Pop two values on the stack, execute a add operation, and push the result
    ldloc.0
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    add
    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)

    // Pop the stack and store it in register 'x'
    ldloc.1
    ldc.i4.s 120
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    callvirt instance void class [System.Private.CoreLib]System.Collections.Generic.Dictionary`2<char, int32>::set_Item(!0, !1)

    // Push the value of register 'x' onto the stack
    ldloc.0
    ldloc.1
    ldc.i4.s 120
    callvirt instance !1 class [System.Private.CoreLib]System.Collections.Generic.Dictionary`2<char, int32>::get_Item(!0)
    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)

    // Push the value of register 'x' onto the stack
    ldloc.0
    ldloc.1
    ldc.i4.s 120
    callvirt instance !1 class [System.Private.CoreLib]System.Collections.Generic.Dictionary`2<char, int32>::get_Item(!0)
    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)

    // Pop two values on the stack, execute a mul operation, and push the result
    ldloc.0
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    mul
    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)

    // Pop the top of the stack and print it
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    call void [System.Console]System.Console::WriteLine(int32)

// Postamble
    ret
}
```

While this may look very complicated, you should notice some repetitive code blocks, like pushing numbers on the stack or retrieving the value in a register.

# Assignment

1. Install .NET 5 [from here](https://dotnet.microsoft.com/en-us/download/dotnet/5.0). _Make sure to install the SDK_, not the Runtime.
1. Install Visual Studio Code (aka `vscode`) [from here](https://code.visualstudio.com/download).
1. Clone this assignment from Github to your local machine. I find [Github Desktop](https://desktop.github.com/) to be handy for this, since Github integrates with it very easily.
1. Open the cloned directory in vscode.
1. When vscode prompts you to install the recommended extensions, do it.
1. Find the two functions that have `TODO` comments. Your job is to fill in the missing code such that the tests all pass.
1. When you have code working, commit it to Github using vscode. Google how to do that if you aren't sure.
1. I'll be able to see if your code passes the tests.

### CIL resources

-   https://weblogs.asp.net/kennykerr/Tags/Introduction%20to%20MSIL, articles 1-2
-   https://en.wikipedia.org/wiki/Common_Intermediate_Language
-   https://en.wikipedia.org/wiki/List_of_CIL_instructions
-   https://sharplab.io

### Some notes

-   Basically, your job is to take the string `3 4 + sx rx rx *` and turn it into that big lump of CIL above--and I've already given you the preamble and postamble.
-   Make use of [sharplab.io](https://sharplab.io) for hints on what CIL instructions to use!
-   The square root function in C# is `double Math.Sqrt(double)`. To convert from `int` to `double`, use the `conv.r8` instruction. To convert from `double` to `int`, use the `conv.ovf.i4` instruction.
-   There are tons of tutorials out there on how to clone from Github and commit your code back to it.
