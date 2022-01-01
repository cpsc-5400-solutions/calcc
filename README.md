# CalcC - a calculator compiler

## Purpose

This project has several purposes:

1. To get your .NET (pronounced "dot net") environment set up
1. To get you used to using `mstest` to test your compiler
1. To build a minimal compiler that targets MSIL/CIL code (the pseudo-assembly language of .NET)
1. To convince you that compilers aren't magic!

## Compilers aren't magic!

They're just _tedious_.

What do I mean by that? The key to understanding and writing compilers is to break things down into tiny, repeatable patterns. Examples of these patterns are:

- _keywords_, like `if` and `for`
- _binary expressions_, like `3 + 3`
- _statements_, like `var x = 3 * y;`

Likewise, when you're writing the output of the compiler (in our case, CIL code), you want to break it down into pieces, too, like:

- the _preamble_, the stuff that you have to put at the top of the file, just because
- the _postamble_, which is just the stuff at the bottom
- _idioms_, which are just little chunks of code that translate directly to something in the source

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

- `+` add
- `-` subtract
- `*` multiply
- `/` divide
- `%` modulus (aka remainder)
- `sqrt` square root

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

CIL (formerly MSIL) is the pseudo-assembly language of .NET. You can easily see what CIL is generated for a C# program at [sharplab.io](https://sharplab.io).

In C#, `3 4 + sx rx rx *` might look like this:

```csharp
using System;
using System.Collections.Generic;

static class Program
{
    static void Main(string[] args)
    {
        var stack = new Stack<int>();
        var registers = new Dictionary<char,int>();
        stack.Push(3);
        stack.Push(4);
        stack.Push(stack.Pop()+stack.Pop());
        registers['x'] = stack.Pop();
        stack.Push(registers['x']);
        stack.Push(registers['x']);
        stack.Push(stack.Pop()*stack.Pop());
        Console.WriteLine(stack.Pop());
    }
}
```

This is simple enough--use a `Stack<int>` to be the calculation stack and a `Dictionary<char,int>` to store the registers.

If you paste that code into [sharplab.io](https://sharplab.io) to see the CIL, **do not be intimidated by the output!** Most of the output is "boilerplate".

Here is what the equivalent CIL would be, with comments to explain what's going on:

```

```
