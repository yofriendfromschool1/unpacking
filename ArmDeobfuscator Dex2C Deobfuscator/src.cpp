#include <iostream>
#include <fstream>
#include <vector>
#include <filesystem>
#include <emmintrin.h>

namespace fs = std::filesystem;

void xorBuffer(char* buffer, size_t size) {
    size_t j = size & 0xFFFFFFE0;
    size_t i = 0;

    for (; i < j; i += 64) {
        __m128i* data1 = (__m128i*) & buffer[i];
        __m128i* data2 = (__m128i*) & buffer[i + 16];
        __m128i* data3 = (__m128i*) & buffer[i + 32];
        __m128i* data4 = (__m128i*) & buffer[i + 48];

        *data1 = _mm_xor_si128(*data1, _mm_set1_epi8(-1));
        *data2 = _mm_xor_si128(*data2, _mm_set1_epi8(-1));
        *data3 = _mm_xor_si128(*data3, _mm_set1_epi8(-1));
        *data4 = _mm_xor_si128(*data4, _mm_set1_epi8(-1));
    }

    for (; i < size; ++i) {
        buffer[i] ^= 0xFF;
    }
}

void deobf(const std::string& inputFile, const std::string& outputFile) {
    std::ifstream inFile(inputFile, std::ios::binary);
    if (!inFile) {
        std::cerr << "Error opening input file" << std::endl;
        return;
    }

    std::ofstream outFile(outputFile, std::ios::binary);
    if (!outFile) {
        std::cerr << "Error opening output file" << std::endl;
        return;
    }

    std::vector<char> buffer(1024);
    while (inFile.read(buffer.data(), buffer.size()) || inFile.gcount() > 0) {
        size_t bytesRead = inFile.gcount();
        xorBuffer(buffer.data(), bytesRead);
        outFile.write(buffer.data(), bytesRead);
    }

    if (outFile.fail()) {
        std::cerr << "An error occured when trying to write to the output file." << std::endl;
    }

    inFile.close();
    outFile.close();

    std::cout << "Sucessfully deobfuscated! Saved in " + outputFile << std::endl;
}

int main() {

    std::string inputDir = "InputDexFiles";
    try {
        for (const auto& entry : fs::directory_iterator(inputDir)) {
            if (entry.is_regular_file() && entry.path().extension() == ".dex") {

                std::string filePath = entry.path().string();
                std::string& inputPath = filePath.replace(inputDir.length(), 1, "/");
                std::string filePath2 = filePath;
                std::string& outputPath = filePath2.replace(0, inputDir.length(), "DeobfuscatedDexFiles");

                deobf(inputPath, outputPath);
            }
        }
    }
    catch (const fs::filesystem_error& e) {
        std::cerr << "Error while opening input dir: " << e.what() << std::endl;
    }
    catch (const std::exception& e) {
        std::cerr << "Error: " << e.what() << std::endl;
    }

    system("pause");

    return 0;
}