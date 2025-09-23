import random

options = ["rock", "paper", "scissors"]

# Adding a scoreboard for a game best of 5
user_score = 0
computer_score = 0
rounds = 1


while True:
    print() # This is a newline 
    print("Welcome to a best of 5! This is Rock, Paper, Scissors game.")
    print(f"Round {rounds}")

    user_choice = input("Enter rock, paper, or scissors (or 'q' to exit): ").lower()

    if user_choice == "q":
        print("Thanks for playing!")
        break

    if user_choice not in options:
        print("Invalid choice. Please try again.")
        continue

    computer_choice = random.choice(options)
    print(f"Computer chose: {computer_choice}")

# Updating the scoreboard
    if user_choice == computer_choice:
        print("It's a tie!")
    elif (user_choice == "rock" and computer_choice == "scissors") or \
         (user_choice == "paper" and computer_choice == "rock") or \
         (user_choice == "scissors" and computer_choice == "paper"):
        print("You won this round!")
        user_score += 1
    else:
        computer_score += 1
        print("Computer won this round!")
    
    print(f"Score - You: {user_score}, computer: {computer_score}")
    print()

    rounds += 1
    if rounds > 5:
        if user_score > computer_score:
            print("Congrats! You won the game!")
        elif user_score < computer_score:
            print("You lost! Better luck next time.")
        else:
            print("It's a tie overall!")
        break


