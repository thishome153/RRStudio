#include "tmain.h"
#include "boost_examples.h"


#include <boost/numeric/ublas/vector.hpp>
#include <boost/numeric/ublas/matrix.hpp>
#include <boost/numeric/ublas/io.hpp>
#include <boost/algorithm/string.hpp>

using  namespace std;
using  namespace boost;
//using  namespace boost::numeric::ublas;


BOOL Boost_MultipleVector()
{
	boost::numeric::ublas::vector<double> x(2);
	
	x(0) = 1; x(1) = 2;
	std::cout << "Vector :" << std::endl;
	std::cout << x << std::endl;

	boost::numeric::ublas::matrix<double> A(2, 2);
	A(0, 0) = 0; A(0, 1) = 1;
	A(1, 0) = 2; A(1, 1) = 3;
	std::cout << "Matrix :" << std::endl;
	std::cout << A << std::endl;

	boost::numeric::ublas::vector <double> y = prod(A, x);
	std::cout << "Result vector :" << std::endl;
	std::cout << y << std::endl;

	return true;
}

int Boost_SplitString(char* str_0, char* Delimiter)
{
	string str1(str_0);
	string str2(Delimiter);
	std::cout << "Source string :"<< str_0 << std::endl;
	std::cout << "Delimiter:" << Delimiter << std::endl;
	typedef vector< iterator_range<string::iterator> > find_vector_type;

	find_vector_type FindVec; // #1: Search for separators
	ifind_all(FindVec, str1, "abc"); // FindVec == { [abc],[ABC],[aBc] }

	typedef vector< string > split_vector_type;

	split_vector_type SplitVec; // #2: Search for tokens
	split(SplitVec, str1, is_any_of(Delimiter), token_compress_on); // SplitVec == { "hello abc","ABC","aBc goodbye" }
	return (int)  SplitVec.size();
}
