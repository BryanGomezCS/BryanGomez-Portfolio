// This is going to be a simple hangman game

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <ctype.h>

//Function Prototypes
const char *get_random_word(const char **category);
void display_word(const char *word, const char *guessed_letters);
int is_word_guessed(const char *word, char *guessed_letters);
int check_guess(const char *word, char *guessed_letters, char guess);

#define MAX_GUESSES 6


int main()
{
    //Making arrays for "categories" that are going to contain words
    const char *animals[] = {"dolphin", "raccoon", "opossum", "fish", "elephant", "giraffe", "lion", "tiger", "bear", "wolf"};
    const char *fruits[] = {"apple", "banana", "orange", "grape", "strawberry", "watermelon", "kiwi", "peach", "pear", "mango"};
    const char *countries[] = {"usa", "canada", "mexico", "brazil", "argentina", "germany", "france", "italy", "spain", "russia"};
    const char *sports[] = {"soccer", "football", "basketball", "baseball", "tennis", "golf", "hockey", "volleyball", "rugby", "cricket"};
    const char *movies[] = {"titanic", "avatar", "inception", "jaws", "rocky", "batman", "the grinch", "toy story", "jurassic park", "star wars"};

    //Making a pointer to a pointer that is going to point to the arrays
    const char **categories[] = {animals, fruits, countries, sports, movies};

    char play_again;

    //Making a variable to store the category
    int category;
    printf("\nWelcome to Hangman!");

    //Asking the user to choose a category
    printf("\nChoose a category to guess the word:\n");
    printf("1. Animals\n2. Fruits\n3. Countries\n4. Sports\n5. Movies\n");
    printf("Enter the number of the category: ");
    scanf("%d", &category);
    category--;  // Adjust for 0-based index

    if (category < 0 || category > 4) {
        printf("Invalid category selection.\n");
        return 1;
    }

    //Getting a random word from the chosen category
    const char *word = get_random_word(categories[category]);
    int word_length = strlen(word);

    //Making array to track the guessed letters (initially all underscores)
    char guessed_letters[word_length + 1];
    for (int i = 0; i < word_length; i++) {
        guessed_letters[i] = '_';
    }
    guessed_letters[word_length] = '\0';

    int tries_left = MAX_GUESSES;
    char guess;
    int guessed_correctly;

    //Game loop
    while (tries_left > 0) {
        printf("\n Word:");
        display_word(word, guessed_letters);

        printf("Tries left: %d\n", tries_left);
        printf("Enter a letter to guess: ");
        scanf(" %c", &guess);
        guess = tolower(guess);

        if(!isalpha(guess)) {
            printf("Invalid input. Please enter a valid letter.\n");
            continue;
        }

        int already_guessed = 0;
        for (int i = 0; i < word_length; i++) {
            if (guessed_letters[i] == guess) {
                already_guessed = 1;
                break;
            }
        }

        if (already_guessed) {
            printf("You already guessed that letter.\n");
            continue;
        }

        //checking if the guessed letter is in the word
        guessed_correctly = check_guess(word, guessed_letters, guess);
        if(!guessed_correctly) {
            tries_left--;
        } else {
            printf("Good Guess\n");
        }

        //checking if the word has been fully guessed
        if (is_word_guessed(word, guessed_letters)) {
            printf("\nCongratulations! You guessed the word: %s\n", word);
            break;
        }
    }
    // If the player runs out of tries
    if (tries_left == 0) {
        printf("\nYou ran out of tries. The word was: %s\n", word);
    }

    //Asking the player if they want to play again
    printf("\nDo you want to play again? (y/n): ");
    scanf(" %c", &play_again);
    if (tolower(play_again) == 'y') {
        main();
    } else {
        printf("Thanks for playing!\n");
    }
    return 0;


}

//Function to get a random word from the chosen category
const char *get_random_word(const char **category)
{
    int num_words = 10;  // Each category has 10 words
    srand(time(NULL));
    return category[rand() % num_words];
}

//Function to display the word with the guessed letters
void display_word(const char *word, const char *guessed_letters)
{
    int word_length = strlen(word);
    for (int i = 0; i < word_length; i++) {
        if (guessed_letters[i] == '_') {
            printf("_ ");
        } else {
            printf("%c ", guessed_letters[i]);
        }
    }
    printf("\n");
}

//Function to check if the word has been guessed
int is_word_guessed(const char *word, char *guessed_letters)
{
    int word_length = strlen(word);
    for (int i = 0; i < word_length; i++) {
        if (guessed_letters[i] == '_') {
            return 0;
        }
    }
    return 1;
}

//Function to check if the guessed letter is in the word
int check_guess(const char *word, char *guessed_letters, char guess)
{
    int guessed_correctly = 0;
    for (int i = 0; i < strlen(word); i++) {
        if (word[i] == guess) {
            guessed_letters[i] = guess;
            guessed_correctly = 1;
        }
    }
    return guessed_correctly;
}



