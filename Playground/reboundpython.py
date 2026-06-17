#function for  adding two numbers
def add_numbers(a, b):
    return a + b

#function for subtracting two numbers
def subtract_numbers(a, b):
    return a - b

#function for multiplying two numbers
def multiply_numbers(a, b):
    return a * b

#function for modulo of two numbers
def mod_numbers(a, b):
    return a % b

#call all the functions and print the results
num1 = 10
num2 = 3

print("Addition:", add_numbers(num1, num2))
print("Subtraction:", subtract_numbers(num1, num2))
print("Multiplication:", multiply_numbers(num1, num2))
print("Modulo:", mod_numbers(num1, num2))