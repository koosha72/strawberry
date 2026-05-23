#ifdef _WIN32
#define TSFDEF __declspec(dllexport)
#else
#define TSFDEF extern
#endif

#define TSF_IMPLEMENTATION
#include "tsf.h"