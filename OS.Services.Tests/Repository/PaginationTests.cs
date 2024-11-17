using OS.Data.Models;
using OS.Data.Repository.Conditions;
using OS.Services.Repository;

namespace OS.Services.Tests.Repository;

public class PaginationTests
{
    private MockRepository _repositoryMock;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new MockRepository();
        RepositoryUtils.PopulateWithBasicData(_repositoryMock);
    }

    [Test]
    public async Task GetListAsync_WithSkip()
    {
        var condition = new CompositeCondition(LogicalSwitch.And);
        condition.Skip = 5;

        var result = await _repositoryMock.GetListAsync<Album>(condition);

        Assert.That(result.Count(), Is.EqualTo(25));
    }

    [Test]
    public async Task GetListAsync_WithTake()
    {
        var condition = new CompositeCondition(LogicalSwitch.And);
        condition.Take = 5;
        var result = await _repositoryMock.GetListAsync<Album>(condition);
        
        Assert.That(result.Count(), Is.EqualTo(5));
    }
    
    [Test]
    public async Task GetListAsync_WithSkipAndTake()
    {
        var condition = new CompositeCondition(LogicalSwitch.And);
        condition.Skip = 5;
        condition.Take = 5;
        var result = await _repositoryMock.GetListAsync<Album>(condition);
        
        Assert.That(result.Count(), Is.EqualTo(5));
    }
    
    [Test]
    public async Task GetListAsync_WithSkipOutOfRange()
    {
        var condition = new CompositeCondition(LogicalSwitch.And);
        condition.Skip = _repositoryMock.Albums.Count + 100;
        var result = await _repositoryMock.GetListAsync<Album>(condition);
        
        Assert.That(result.Count(), Is.EqualTo(0));
    }
    
    [Test]
    public async Task GetListAsync_WithTakeOutOfRange()
    {
        var condition = new CompositeCondition(LogicalSwitch.And);
        condition.Take = _repositoryMock.Albums.Count + 100;
        var result = await _repositoryMock.GetListAsync<Album>(condition);
        
        Assert.That(result.Count(), Is.EqualTo(_repositoryMock.Albums.Count));
    }
}