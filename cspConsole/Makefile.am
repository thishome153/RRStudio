## \brief �������� �!"�����������" ������ ������ � "��������� CSP"
##       � �������� ��������� �������� ����� CSP. ��� �������������
##       "��������� CSP" � ������������� ������������ 
##       (/opt/CPROcsp/{include,lib}) ������� ������������ csptest.prd
##

bin_PROGRAMS=csptest
PROJECT=csptest
## DEBUG=debug

## project: bin_PROGRAMS

INCLUDES=-I../../src/include

csptest_SOURCES = base64.c ctkey.c getop1.c getopt.c tmain.c
##       stress.c

csptest_LDADD=$(TGT_DIR)/libcsp.so
$(TGT_DIR)/libcsp.so:

@make_love@