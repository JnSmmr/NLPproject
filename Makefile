# Makefile

# Set the C# compiler to use
CSC = dotnet

# Set the output directory for the compiled files
OUTPUT_DIR = ./bin


all: Program

Program: NLPproject.csproj
	$(CSC) build $(BUILD_FLAGS) -o $(OUTPUT_DIR) NLPproject.csproj


clean:
	rm -rf $(OUTPUT_DIR)