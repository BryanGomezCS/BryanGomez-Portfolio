#include <stdio.h>
#include <stdlib.h>
#include <time.h>

int main()
{

    int min_value = 1, max_value = 100;
    int random_number, guess, attempts;
    char play_again;

    srand(time(NULL));

    do
    {

        random_number = rand() % (max_value - min_value + 1) + min_value;
        attempts = 0;

        printf("\nWelcome to the Guess the Number Game!\n");
        printf("I have selected a number between %d and %d\n", min_value, max_value);
        printf("Can you guess what it is?\n");

        do
        {
            printf("Enter your guess: ");
            scanf("%d", &guess);
            attempts++;

            if (guess < random_number)
            {
                printf("Too low! Try again.\n");
            }
            else if (guess > random_number)
            {
                printf("Too high! Try again.\n");
            }
            else
            {
                printf("Congrats! You got it!\n");
                printf("It took you %d attempts\n", attempts);
            }
        } while (guess != random_number);
        
            printf("You do want to play again?(y/n): ");
            scanf(" %c", &play_again);
        
    } while (play_again == 'y' || play_again == 'Y');

    printf("\nThanks for playing!");

    return 0;
}
